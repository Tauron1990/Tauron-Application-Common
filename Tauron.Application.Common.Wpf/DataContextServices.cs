#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The data context changing complete attribute.</summary>
    [AttributeUsage(AttributeTargets.Method)]
    [DebuggerNonUserCode]
    [PublicAPI]
    public sealed class DataContextChangingCompleteAttribute : Attribute
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DataContextChangingCompleteAttribute" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="DataContextChangingCompleteAttribute" /> Klasse.
        /// </summary>
        public DataContextChangingCompleteAttribute()
        {
            Sync = true;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets a value indicating whether sync.</summary>
        public bool Sync { get; set; }

        #endregion
    }

    [PublicAPI]
    [ContractClass(typeof (PipeLineContracts))]
    internal interface IPipeLine : IWeakReference
    {
        #region Public Properties

        /// <summary>Gets or sets the data context.</summary>
        [CanBeNull]
        WeakReference DataContext { get; set; }

        /// <summary>Gets or sets the task scheduler.</summary>
        [CanBeNull]
        TaskScheduler TaskScheduler { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The generate.
        /// </summary>
        /// <param name="dataContext">
        ///     The data context.
        /// </param>
        /// <param name="scheduler">
        ///     The scheduler.
        /// </param>
        /// <returns>
        ///     The <see cref="ITask" />.
        /// </returns>
        [NotNull]
        ITask Generate([NotNull] WeakReference dataContext, [NotNull] TaskScheduler scheduler);

        #endregion
    }

    [ContractClassFor(typeof (IPipeLine))]
    internal abstract class PipeLineContracts : IPipeLine
    {
        #region Public Properties

        /// <summary>Gets or sets the data context.</summary>
        /// <exception cref="NotImplementedException"></exception>
        public WeakReference DataContext
        {
            get { throw new NotImplementedException(); }

            set { throw new NotImplementedException(); }
        }

        /// <summary>Gets a value indicating whether is alive.</summary>
        /// <exception cref="NotImplementedException"></exception>
        public bool IsAlive
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>Gets or sets the task scheduler.</summary>
        /// <exception cref="NotImplementedException"></exception>
        public TaskScheduler TaskScheduler
        {
            get { throw new NotImplementedException(); }

            set { throw new NotImplementedException(); }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The generate.
        /// </summary>
        /// <param name="dataContext">
        ///     The data context.
        /// </param>
        /// <param name="scheduler">
        ///     The scheduler.
        /// </param>
        /// <returns>
        ///     The <see cref="ITask" />.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public ITask Generate(WeakReference dataContext, TaskScheduler scheduler)
        {
            Contract.Requires<ArgumentNullException>(dataContext != null, "dataContext");
            Contract.Requires<ArgumentNullException>(scheduler != null, "scheduler");

            Contract.Ensures(Contract.Result<ITask>() != null);

            throw new NotImplementedException();
        }

        #endregion
    }

    internal abstract class PipelineBase : IPipeLine, ITask
    {
        #region Fields

        private readonly WeakReference<DependencyObject> _element;

        private readonly TaskCompletionSource<object> _task;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="PipelineBase" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="PipelineBase" /> Klasse.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        protected PipelineBase([NotNull] DependencyObject target)
        {
            _element = new WeakReference<DependencyObject>(target);
            DataContextServices.RegisterHandler(target, this);
            _task = new TaskCompletionSource<object>();
            _task.SetResult(null);
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the source.</summary>
        [CanBeNull]
        public WeakReference<DependencyObject> Source
        {
            get { return _element; }
        }

        /// <summary>Gets the target.</summary>
        [CanBeNull]
        public DependencyObject Target
        {
            get { return _element.TypedTarget(); }
        }

        /// <summary>Gets or sets the data context.</summary>
        public WeakReference DataContext { get; set; }

        /// <summary>Gets a value indicating whether is alive.</summary>
        public bool IsAlive
        {
            get { return _element.IsAlive(); }
        }

        /// <summary>Gets or sets the task scheduler.</summary>
        [NotNull]
        public TaskScheduler TaskScheduler { get; set; }

        /// <summary>Gets a value indicating whether synchronize.</summary>
        public virtual bool Synchronize
        {
            get { return true; }
        }

        /// <summary>Gets the task.</summary>
        [NotNull]
        public Task Task
        {
            get { return _task.Task; }
        }

        #endregion

        #region Explicit Interface Methods

        ITask IPipeLine.Generate(WeakReference dataContext, TaskScheduler scheduler)
        {
            DataContext = dataContext;
            TaskScheduler = scheduler;
            return this;
        }

        void ITask.Execute()
        {
            DataContextChanged();
        }

        #endregion

        #region Methods

        /// <summary>The data context changed.</summary>
        protected virtual void DataContextChanged()
        {
        }

        #endregion
    }

    // [DebuggerNonUserCode]
    /// <summary>The data context services.</summary>
    [PublicAPI]
    public static class DataContextServices
    {
        #region Static Fields

        public static readonly DependencyProperty ActivateProperty = DependencyProperty.RegisterAttached(
            "Activate",
            typeof (bool),
            typeof (
                DataContextServices
                ),
            new UIPropertyMetadata
                (false,
                 OnActivateChanged));

        public static readonly DependencyProperty DataContextProperty =
            DependencyProperty.RegisterAttached(
                "DataContext",
                typeof (object),
                typeof (DataContextServices),
                new FrameworkPropertyMetadata(null,
                                              FrameworkPropertyMetadataOptions.Inherits,
                                              BeginDataContextChanging));

        private static readonly WeakReferenceCollection<ObjectReference> Objects =
            new WeakReferenceCollection<ObjectReference>();

        private static readonly WeakReferenceCollection<RequestingElement> RequestingCollection =
            new WeakReferenceCollection<RequestingElement>();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The get activate.
        /// </summary>
        /// <param name="obj">
        ///     The obj.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool GetActivate([NotNull] DependencyObject obj)
        {
            Contract.Requires<ArgumentNullException>(obj != null, "obj");

            return (bool) obj.GetValue(ActivateProperty);
        }

        /// <summary>
        ///     The get data context.
        /// </summary>
        /// <param name="obj">
        ///     The obj.
        /// </param>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        [CanBeNull]
        public static object GetDataContext([NotNull] DependencyObject obj)
        {
            Contract.Requires<ArgumentNullException>(obj != null, "obj");

            return obj.GetValue(DataContextProperty);
        }

        /// <summary>
        ///     The set activate.
        /// </summary>
        /// <param name="obj">
        ///     The obj.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        public static void SetActivate([NotNull] DependencyObject obj, bool value)
        {
            Contract.Requires<ArgumentNullException>(obj != null, "obj");

            obj.SetValue(ActivateProperty, value);
        }

        /// <summary>
        ///     The set data context.
        /// </summary>
        /// <param name="obj">
        ///     The obj.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        public static void SetDataContext([NotNull] DependencyObject obj, [CanBeNull] object value)
        {
            Contract.Requires<ArgumentNullException>(obj != null, "obj");

            obj.SetValue(ActivateProperty, value);
        }

        /// <summary>
        ///     The trigger rebind.
        /// </summary>
        /// <param name="obj">
        ///     The obj.
        /// </param>
        public static void TriggerRebind([NotNull] DependencyObject obj)
        {
            Contract.Requires<ArgumentNullException>(obj != null, "obj");

            ObjectReference objRef = FindObjectRecusiv(obj);
            if (objRef == null) return;

            DependencyObject depObj = objRef.DependencyObject;
            if (depObj == null) return;

            objRef.NewDataContext(FindDataContext(obj), CommonApplication.Scheduler);
        }

        #endregion

        #region Methods

        internal static void Activate([NotNull] DependencyObject element)
        {
            ObjectReference objRef = FindObject(element);
            if (objRef != null) return;

            objRef = new ObjectReference(element);
            Objects.Add(objRef);

            new FrameworkObject(element, false).DataContextChanged += BeginDataContextChanging;
        }

        internal static void Deactivate([NotNull] DependencyObject element)
        {
            ObjectReference objRef = FindObject(element);
            if (objRef == null) return;

            Objects.Remove(objRef);

            new FrameworkObject(element).DataContextChanged -= BeginDataContextChanging;
        }

        [CanBeNull]
        internal static object FindDataContext([CanBeNull] DependencyObject obj)
        {
            if (obj == null) return null;

            object result = new FrameworkObject(obj, false).DataContext ?? obj.GetValue(DataContextProperty);

            return result;
        }

        internal static bool RegisterHandler([CanBeNull] DependencyObject element, [NotNull] IPipeLine pipline)
        {
            ObjectReference objRef = FindObjectRecusiv(element);

            if (objRef != null)
            {
                RegisterCore(objRef, pipline);
                return true;
            }

            RegisterForRequesting(element, pipline);
            return false;
        }

        private static void BeginDataContextChanging([NotNull] object d, DependencyPropertyChangedEventArgs e)
        {
            ObjectReference objRef = FindObject(d);
            if (objRef == null) return;

            objRef.NewDataContext(e.NewValue, CommonApplication.Scheduler);
        }

        [CanBeNull,DebuggerStepThrough]
        private static ObjectReference FindObject([CanBeNull] object obj)
        {
            return Objects.FirstOrDefault(@ref => @ref.IsMatch(obj));
        }

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

            var after = e.NewValue.CastObj<bool>();
            var before = e.OldValue.CastObj<bool>();
            if (after && before) return;

            if (before) Deactivate(d);

            if (!before && after) Activate(d);
        }

        private static void RegisterCore([NotNull] ObjectReference reference, [NotNull] IPipeLine pipline)
        {
            reference.AddPipline(pipline, CommonApplication.Scheduler);
        }

        private static void RegisterForRequesting([CanBeNull] DependencyObject obj, [NotNull] IPipeLine pipline)
        {
            RequestingCollection.Add(new RequestingElement(obj, pipline));
        }

        private static void TriggerRebind([CanBeNull] DependencyObject obj, [NotNull] IPipeLine pipline)
        {
            ObjectReference refernence = FindObjectRecusiv(obj);
            if (refernence == null) return;

            object dataContext = FindDataContext(obj);
            if (dataContext == null) return;

            CommonApplication.Scheduler.QueueTask(
                pipline.Generate(new WeakReference(dataContext),
                                 CommonApplication.Scheduler));
        }

        #endregion

        // [DebuggerNonUserCode]
        private class ObjectReference : IWeakReference
        {
            #region Fields

            private readonly List<IPipeLine> _pips;

            private readonly WeakReference<DependencyObject> _target;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="ObjectReference" /> class.
            ///     Initialisiert eine neue Instanz der <see cref="ObjectReference" /> Klasse.
            /// </summary>
            /// <param name="obj">
            ///     The obj.
            /// </param>
            public ObjectReference([NotNull] DependencyObject obj)
            {
                _pips = new List<IPipeLine>();
                _target = new WeakReference<DependencyObject>(obj);
            }

            #endregion

            #region Public Properties

            /// <summary>Gets the dependency object.</summary>
            [CanBeNull]
            public DependencyObject DependencyObject
            {
                get { return _target.TypedTarget(); }
            }

            /// <summary>Gets a value indicating whether is alive.</summary>
            public bool IsAlive
            {
                get { return _target.IsAlive(); }
            }

            #endregion

            #region Public Methods and Operators

            /// <summary>
            ///     The add pipline.
            /// </summary>
            /// <param name="pipline">
            ///     The pipline.
            /// </param>
            /// <param name="schedule">
            ///     The schedule.
            /// </param>
            public void AddPipline([NotNull] IPipeLine pipline, [NotNull] TaskScheduler schedule)
            {
                object context = FindDataContext(DependencyObject);
                if (context != null)
                {
                    pipline.DataContext = new WeakReference(context);
                    pipline.TaskScheduler = schedule;
                }

                _pips.Add(pipline);
            }

            /// <summary>
            ///     The is match.
            /// </summary>
            /// <param name="obj">
            ///     The obj.
            /// </param>
            /// <returns>
            ///     The <see cref="bool" />.
            /// </returns>
            public bool IsMatch([CanBeNull] object obj)
            {
                return ReferenceEquals(_target.TypedTarget(), obj);
            }

            /// <summary>
            ///     The new data context.
            /// </summary>
            /// <param name="dataContext">
            ///     The data context.
            /// </param>
            /// <param name="scheduler">
            ///     The scheduler.
            /// </param>
            public void NewDataContext([CanBeNull] object dataContext, [NotNull] TaskScheduler scheduler)
            {
                var weakDataContext = new WeakReference(dataContext);
                foreach (IPipeLine pip in _pips.ToArray()) scheduler.QueueTask(pip.Generate(weakDataContext, scheduler));

                if(dataContext == null) return;

                Type type = dataContext.GetType();

                IEnumerable<Tuple<MemberInfo, DataContextChangingCompleteAttribute>> temp =
                    type.FindMemberAttributes<DataContextChangingCompleteAttribute>(true);

                Tuple<MemberInfo, DataContextChangingCompleteAttribute> memInfo = temp.FirstOrDefault();
                if (memInfo == null) return;

                scheduler.QueueTask(
                    new DataChangingCompledTask(memInfo.Item1.CastObj<MethodInfo>(), dataContext,
                                                memInfo.Item2.Sync));
            }

            #endregion

            private class DataChangingCompledTask : ITask
            {
                #region Fields

                private readonly object _dataContext;

                private readonly MethodInfo _info;

                private readonly bool _sync;

                private readonly TaskCompletionSource<object> _task;

                #endregion

                #region Constructors and Destructors

                /// <summary>
                ///     Initializes a new instance of the <see cref="DataChangingCompledTask" /> class.
                ///     Initialisiert eine neue Instanz der <see cref="DataChangingCompledTask" /> Klasse.
                /// </summary>
                /// <param name="info">
                ///     The info.
                /// </param>
                /// <param name="dataContext">
                ///     The data context.
                /// </param>
                /// <param name="sync">
                ///     The sync.
                /// </param>
                public DataChangingCompledTask([NotNull] MethodInfo info, [NotNull] object dataContext, bool sync)
                {
                    _sync = sync;
                    _info = info;
                    _dataContext = dataContext;
                    _task = new TaskCompletionSource<object>();
                    _task.SetResult(null);
                }

                #endregion

                #region Public Properties

                /// <summary>Gets a value indicating whether synchronize.</summary>
                public bool Synchronize
                {
                    get { return _sync; }
                }

                /// <summary>Gets the task.</summary>
                [NotNull]
                public Task Task
                {
                    get { return _task.Task; }
                }

                #endregion

                #region Public Methods and Operators

                /// <summary>The execute.</summary>
                public void Execute()
                {
                    _info.Invoke(_dataContext);
                }

                #endregion
            }
        }

        private class RequestingElement : IWeakReference
        {
            #region Fields

            private readonly FrameworkObject _depObj;

            private readonly IPipeLine _pipline;

            private bool _isClear;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="RequestingElement" /> class.
            ///     Initialisiert eine neue Instanz der <see cref="RequestingElement" /> Klasse.
            /// </summary>
            /// <param name="obj">
            ///     The obj.
            /// </param>
            /// <param name="pipline">
            ///     The pipline.
            /// </param>
            public RequestingElement([CanBeNull] DependencyObject obj, [NotNull] IPipeLine pipline)
            {
                _depObj = new FrameworkObject(obj);
                _pipline = pipline;

                _depObj.LoadedEvent += LoadedEventHandler;
            }

            #endregion

            #region Explicit Interface Properties

            bool IWeakReference.IsAlive
            {
                get { return !_isClear || ((IWeakReference) _depObj).IsAlive; }
            }

            #endregion

            #region Methods

            private void LoadedEventHandler([NotNull] object sender, [NotNull] RoutedEventArgs e)
            {
                _depObj.LoadedEvent -= LoadedEventHandler;
                _isClear = true;
                if (RegisterHandler(_depObj.Original, _pipline)) TriggerRebind(_depObj.Original, _pipline);
            }

            #endregion
        }
    }
}