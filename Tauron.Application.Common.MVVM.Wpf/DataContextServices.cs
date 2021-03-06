﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using JetBrains.Annotations;

namespace Tauron.Application
{
    [AttributeUsage(AttributeTargets.Method)]
    [DebuggerNonUserCode]
    [PublicAPI]
    public sealed class DataContextChangingCompleteAttribute : Attribute
    {
        public DataContextChangingCompleteAttribute() => Sync = true;

        public bool Sync { get; set; }
        
    }

    [PublicAPI]
    internal interface IPipeLine : IWeakReference
    {

        [NotNull]
        ITask Generate([NotNull] WeakReference dataContext, [NotNull] ITaskScheduler scheduler);
        
        [CanBeNull]
        WeakReference DataContext { get; set; }
        
        [CanBeNull]
        ITaskScheduler TaskScheduler { get; set; }
    }
    
    [PublicAPI]
    public static class DataContextServices
    {
        private class ObjectReference : IWeakReference
        {

            public ObjectReference([NotNull] DependencyObject obj)
            {
                _pips = new List<IPipeLine>();
                _target = new WeakReference<DependencyObject>(Argument.NotNull(obj, nameof(obj)));
            }
            

            private class DataChangingCompledTask : ITask
            {
                public DataChangingCompledTask([NotNull] MethodInfo info, [NotNull] object dataContext, bool sync)
                {
                    Synchronize = sync;
                    _info = Argument.NotNull(info, nameof(info));
                    _dataContext = Argument.NotNull(dataContext, nameof(dataContext));
                    _task = new TaskCompletionSource<object>();
                    _task.SetResult(null);
                }
                
                public void Execute() => _info.InvokeFast(_dataContext);

                private readonly object _dataContext;

                private readonly MethodInfo _info;

                private readonly TaskCompletionSource<object> _task;
                
                public bool Synchronize { get; }
                
                public Task Task => _task.Task;
                
            }
            
            private readonly List<IPipeLine> _pips;

            private readonly WeakReference<DependencyObject> _target;
            
            [CanBeNull]
            public DependencyObject DependencyObject => _target.TypedTarget();
            
            public bool IsAlive => _target.IsAlive();
            
            public void AddPipline([NotNull] IPipeLine pipline, [NotNull] ITaskScheduler schedule)
            {
                var context = FindDataContext(DependencyObject);
                if (context != null)
                {
                    pipline.DataContext = new WeakReference(context);
                    pipline.TaskScheduler = schedule;
                }

                if (_pips.Contains(pipline)) return;

                _pips.Add(pipline);
            }

            public void RemovePipline(IPipeLine pipeline) => _pips.Remove(pipeline);

            public bool IsMatch([CanBeNull] object obj) => ReferenceEquals(_target.TypedTarget(), obj);

            public void NewDataContext([CanBeNull] object dataContext, [NotNull] ITaskScheduler scheduler)
            {
                Argument.NotNull(scheduler, nameof(scheduler));

                var weakDataContext = new WeakReference(dataContext);
                foreach (var pip in _pips.ToArray())
                    scheduler.QueueTask(pip.Generate(weakDataContext, scheduler));

                var type = dataContext?.GetType();

                var temp = type?.FindMemberAttributes<DataContextChangingCompleteAttribute>(true);

                var memInfo = temp?.FirstOrDefault();
                if (memInfo == null) return;

                scheduler.QueueTask(new DataChangingCompledTask(memInfo.Item1.SafeCast<MethodInfo>(), dataContext, memInfo.Item2.Sync));
            }
        }

        private class RequestingElement : IWeakReference
        {
            public RequestingElement([NotNull] DependencyObject obj, [NotNull] IPipeLine pipline)
            {
                _depObj = new FrameworkObject(obj);
                _pipline = pipline;

                _depObj.LoadedEvent += LoadedEventHandler;
            }
            
            bool IWeakReference.IsAlive => !_isClear || ((IWeakReference) _depObj).IsAlive;
            
            private void LoadedEventHandler([NotNull] object sender, [NotNull] RoutedEventArgs e)
            {
                _depObj.LoadedEvent -= LoadedEventHandler;
                _isClear = true;
                if (RegisterHandler(Argument.CheckResult(_depObj.Original, $"{nameof(RequestingElement)} -- Original Element Was Null"), _pipline)) TriggerRebind(_depObj.Original, _pipline);
            }
            
            private readonly FrameworkObject _depObj;

            private readonly IPipeLine _pipline;

            private bool _isClear;
            
        }
        
        public static readonly DependencyProperty ActivateProperty = DependencyProperty.RegisterAttached("Activate", typeof(bool), typeof(DataContextServices), new UIPropertyMetadata(false, OnActivateChanged));

        public static readonly DependencyProperty DataContextProperty =
            DependencyProperty.RegisterAttached(
                "DataContext",
                typeof(object),
                typeof(DataContextServices),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.Inherits,
                    BeginDataContextChanging));

        private static readonly WeakReferenceCollection<ObjectReference> Objects =
            new WeakReferenceCollection<ObjectReference>();

        private static readonly WeakReferenceCollection<RequestingElement> RequestingCollection =
            new WeakReferenceCollection<RequestingElement>();
        
        public static bool GetActivate([NotNull] DependencyObject obj) => (bool) Argument.NotNull(obj, nameof(obj)).GetValue(ActivateProperty);

        [CanBeNull]
        public static object GetDataContext([NotNull] DependencyObject obj) => Argument.NotNull(obj, nameof(obj)).GetValue(DataContextProperty);

        public static void SetActivate([NotNull] DependencyObject obj, bool value) => Argument.NotNull(obj, nameof(obj)).SetValue(ActivateProperty, value);

        public static void SetDataContext([NotNull] DependencyObject obj, [CanBeNull] object value) => Argument.NotNull(obj, nameof(obj)).SetValue(ActivateProperty, value);

        public static void TriggerRebind([NotNull] DependencyObject obj)
        {
            var objRef = FindObjectRecusiv(Argument.NotNull(obj, nameof(obj)));

            var depObj = objRef?.DependencyObject;
            if (depObj == null) return;

            objRef.NewDataContext(FindDataContext(obj), CommonApplication.Scheduler);
        }

        internal static void Activate([NotNull] DependencyObject element)
        {
            var objRef = FindObject(element);
            if (objRef != null) return;

            objRef = new ObjectReference(element);
            Objects.Add(objRef);

            new FrameworkObject(element, false).DataContextChanged += BeginDataContextChanging;
        }

        internal static void Deactivate([NotNull] DependencyObject element)
        {
            var objRef = FindObject(element);
            if (objRef == null) return;

            Objects.Remove(objRef);

            new FrameworkObject(element).DataContextChanged -= BeginDataContextChanging;
        }

        [CanBeNull]
        internal static object FindDataContext([CanBeNull] DependencyObject obj)
        {
            if (obj == null) return null;

            var result = new FrameworkObject(obj, false).DataContext ?? obj.GetValue(DataContextProperty);

            return result;
        }

        internal static bool RegisterHandler([NotNull] DependencyObject element, [NotNull] IPipeLine pipline)
        {
            var objRef = FindObjectRecusiv(element);

            if (objRef != null)
            {
                RegisterCore(objRef, pipline);
                return true;
            }

            RegisterForRequesting(element, pipline);
            return false;
        }

        internal static void UnregisterHandler([CanBeNull] DependencyObject element, [NotNull] IPipeLine pipeLine)
        {
            var objRef = FindObjectRecusiv(element);

            objRef?.RemovePipline(pipeLine);
        }

        private static void BeginDataContextChanging([NotNull] object d, DependencyPropertyChangedEventArgs e)
        {
            var objRef = FindObject(d);

            objRef?.NewDataContext(e.NewValue, CommonApplication.Scheduler);
        }

        [CanBeNull]
        [DebuggerStepThrough]
        private static ObjectReference FindObject([CanBeNull] object obj) => Objects.FirstOrDefault(@ref => @ref.IsMatch(obj));

        [CanBeNull]
        private static ObjectReference FindObjectRecusiv([CanBeNull] DependencyObject element)
        {
            ObjectReference objRef;

            do
            {
                objRef = FindObject(element);
                if (objRef != null) break;

                var temp = new FrameworkObject(element, false);
                element = temp.Parent ?? temp.VisualParent;
            } while (element != null);

            return objRef;
        }

        private static void OnActivateChanged([NotNull] DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(d)) return;

            var after = e.NewValue.SafeCast<bool>();
            var before = e.OldValue.SafeCast<bool>();
            if (after && before) return;

            if (before) Deactivate(d);

            if (!before && after) Activate(d);
        }

        private static void RegisterCore([NotNull] ObjectReference reference, [NotNull] IPipeLine pipline) => reference.AddPipline(pipline, CommonApplication.Scheduler);

        private static void RegisterForRequesting([NotNull] DependencyObject obj, [NotNull] IPipeLine pipline) => RequestingCollection.Add(new RequestingElement(obj, pipline));

        private static void TriggerRebind([CanBeNull] DependencyObject obj, [NotNull] IPipeLine pipline)
        {
            var refernence = FindObjectRecusiv(obj);
            if (refernence == null) return;

            var dataContext = FindDataContext(obj);
            if (dataContext == null) return;

            CommonApplication.Scheduler.QueueTask(
                pipline.Generate(new WeakReference(dataContext),
                    CommonApplication.Scheduler));
        }
    }
}