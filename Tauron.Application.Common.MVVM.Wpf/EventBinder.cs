using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using JetBrains.Annotations;
using Tauron.Application.Commands;
using Tauron.Application.Views;

namespace Tauron.Application
{
    [PublicAPI]
    public static class EventBinder
    {
        private static void OnEventsChanged([NotNull] DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(d)) return;

            var simpleMode = false;

            if (!(e.NewValue is string newValue)) return;

            if (newValue.StartsWith("SimpleMode"))
            {
                simpleMode = true;
                newValue = newValue.Remove(0, 10);
            }

            if (e.OldValue is string oldValue)
            {
                foreach (var linker in
                    EventLinkerCollection.Where(ele => ele.Commands == oldValue && Equals(ele.Target, d)))
                {
                    linker.Commands = newValue;
                    linker.SimpleMode = simpleMode;
                    linker.Bind();
                    return;
                }
            }

            var temp = new EventLinker(newValue, d, simpleMode);
            EventLinkerCollection.Add(temp);
            temp.Bind();
        }
        
        private sealed class EventLinker : PipelineBase, IDisposable
        {
            private readonly List<InternalEventLinker> _linkers = new List<InternalEventLinker>();

            public EventLinker([NotNull] string commands, [NotNull] DependencyObject target, bool simpleMode)
                : base(target, simpleMode) => Commands = Argument.NotNull(commands, nameof(commands));

            [CanBeNull]
            public string Commands { get; set; }

            internal class CommandMember
            {
                public CommandMember([NotNull] string name, [NotNull] MemberInfo memberInfo, bool synchronize, [CanBeNull] Type converter)
                {
                    Name = name;
                    MemberInfo = memberInfo;
                    Synchronize = synchronize;
                    Converter = converter;
                }

                [NotNull]
                public MemberInfo MemberInfo { get; }

                [NotNull]
                public string Name { get; }

                public bool Synchronize { get; }

                [CanBeNull]
                public Type Converter { get; }
            }

            //[DebuggerNonUserCode]
            private class InternalEventLinker : IDisposable
            {
             
                private static readonly MethodInfo Method = typeof(InternalEventLinker).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                        .First(m => m.Name == "Handler");

                public InternalEventLinker([CanBeNull] IEnumerable<CommandMember> member, [CanBeNull] EventInfo @event, [NotNull] WeakReference dataContext,
                    [NotNull] string targetName, [CanBeNull] WeakReference<DependencyObject> host, [NotNull] ITaskScheduler scheduler)
                {
                    _isDirty = (member == null) | (@event == null);

                    _scheduler = scheduler;
                    _host = host;
                    _event = @event;
                    _dataContext = dataContext;
                    _targetName = targetName;
                    FindMember(member);

                    Initialize();
                }

                public void Dispose()
                {
                    object host = _host.TypedTarget();
                    if (host == null || _delegate == null) return;

                    _event.RemoveEventHandler(host, _delegate);
                    _delegate = null;
                }

                private readonly EventInfo _event;
                private readonly WeakReference _dataContext;
                private readonly WeakReference<DependencyObject> _host;
                private readonly ITaskScheduler _scheduler;
                private readonly string _targetName;
                private Delegate _delegate;
                private ICommand _command;
                private bool _isDirty;
                private MemberInfo _member;
                private bool _sync;
                private ISimpleConverter _simpleConverter;

                private bool EnsureCommandStade()
                {
                    if (_command != null) return true;

                    if (_member == null) return false;

                    try
                    {
                        var context = _dataContext.Target;
                        if (context == null) return false;

                        var minfo = _member as MethodInfo;
                        if (minfo != null)
                        {
                            if (minfo.ReturnType.IsAssignableFrom(typeof(ICommand))) _command = (ICommand) minfo.Invoke(context, null);
                            else _command = new MethodCommand(minfo, _dataContext);
                        }
                        else
                        {
                            var pifno = _member as PropertyInfo;
                            if (pifno != null) _command = (ICommand) pifno.GetValueFast(context);
                            else _command = (ICommand) ((FieldInfo) _member).GetValueFast(context);
                        }
                    }
                    catch (InvalidCastException e)
                    {
                        CommonWpfConstans.LogCommon(true, "EventBinder: Casting Faild: {0}|{1}|{2}", _dataContext.Target,
                            _targetName, e);
                        _isDirty = true;
                    }
                    catch (ArgumentException e)
                    {
                        CommonWpfConstans.LogCommon(true, "EventBinder: invalid Argument: {0}|{1}|{2}", _dataContext.Target,
                            _targetName, e);

                        _isDirty = true;
                    }

                    return _command != null && !_isDirty;
                }

                private void FindMember([CanBeNull] IEnumerable<CommandMember> members)
                {
                    if (members == null)
                    {
                        CommonWpfConstans.LogCommon(false, "EventBinder: No Members: {0}", _dataContext.Target);
                        return;
                    }

                    var temp = members.FirstOrDefault(mem => mem.Name == _targetName);
                    if (temp == null)
                    {
                        CommonWpfConstans.LogCommon(false, "EventBinder: No Valid Member found: {0}|{1}",
                            _dataContext.Target, _targetName);
                        return;
                    }

                    _member = temp.MemberInfo;
                    _sync = temp.Synchronize;
                    try
                    {
                        _simpleConverter = temp.Converter != null ? temp.Converter.FastCreateInstance() as ISimpleConverter : null;
                    }
                    catch (Exception e)
                    {
                        if (e.IsCriticalApplicationException())
                            throw;
                    }
                }

                [UsedImplicitly]
                private void Handler([NotNull] object sender, [NotNull] EventArgs e)
                {
                    if (!_isDirty && !EnsureCommandStade())
                    {
                        Dispose();
                        return;
                    }

                    try
                    {
                        _scheduler.QueueTask(
                            new UserTask(() =>
                                {
                                    object localSender = _simpleConverter?.Convert(sender) ?? sender;
                                    object localEventArgs = _simpleConverter?.Convert(e) ?? e;

                                    var data = new EventData(localSender, localEventArgs);
                                    if (_command.CanExecute(data))
                                        _command.Execute(data);
                                },
                                _sync));
                    }
                    catch (ArgumentException)
                    {
                        _isDirty = true;
                    }
                }

                private void Initialize()
                {
                    if (_isDirty || _event == null) return;

                    object typedTarget = _host.TypedTarget();
                    if (typedTarget == null) return;

                    _delegate = Delegate.CreateDelegate(_event.EventHandlerType, this, Method);
                    _event.AddEventHandler(typedTarget, _delegate);
                }

            }

            public void Dispose() => Free();

            public void Bind() => DataContextChanged();

            protected override void DataContextChanged()
            {
                Free();

                if (Commands == null)
                {
                    CommonWpfConstans.LogCommon(false, "EventBinder: No Command Setted: {0}",
                        DataContext == null ? "Unkowen" : DataContext.Target);

                    return;
                }

                var vals = Commands.Split(':');
                var events = new Dictionary<string, string>();

                try
                {
                    for (var i = 0; i < vals.Length; i++) events[vals[i]] = vals[++i];
                }
                catch (IndexOutOfRangeException)
                {
                    CommonWpfConstans.LogCommon(false, "EventBinder: EventPairs not Valid: {0}", Commands);
                }

                if (DataContext == null) return;

                var dataContext = DataContext.Target;
                var host = Target;
                if (host == null || dataContext == null) return;

                var hostType = host.GetType();
                var dataContextInfos =
                    (from entry in dataContext.GetType().FindMemberAttributes<EventTargetAttribute>(true)
                        select new CommandMember(
                                entry.Item2.ProvideMemberName(entry.Item1),
                                entry.Item1,
                                entry.Item2.Synchronize,
                                entry.Item2.Converter)).ToArray();

                foreach (var pair in events)
                {
                    var info = hostType.GetEvent(pair.Key);
                    if (info == null)
                    {
                        CommonWpfConstans.LogCommon(false, "EventBinder: No event Found: {0}|{1}", hostType, pair.Key);
                        return;
                    }

                    _linkers.Add(
                        new InternalEventLinker(
                            dataContextInfos,
                            info,
                            DataContext,
                            pair.Value,
                            Source,
                            TaskScheduler ?? throw new InvalidOperationException()));
                }
            }

            private void Free()
            {
                foreach (var linker in _linkers) linker.Dispose();

                _linkers.Clear();
            }

        }

        public static readonly DependencyProperty EventsProperty = 
            DependencyProperty.RegisterAttached("Events", typeof(string), typeof(EventBinder), new UIPropertyMetadata(null, OnEventsChanged));

        private static readonly WeakReferenceCollection<EventLinker> EventLinkerCollection = new WeakReferenceCollection<EventLinker>();

        [NotNull]
        public static string GetEvents([NotNull] DependencyObject obj) => (string) Argument.NotNull(obj, nameof(obj)).GetValue(EventsProperty);

        public static void SetEvents([NotNull] DependencyObject obj, [NotNull] string value) => Argument.NotNull(obj, nameof(obj)).SetValue(EventsProperty, Argument.NotNull(value, nameof(value)));
    }
}