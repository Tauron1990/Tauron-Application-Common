using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    public class EventManager : IEventManager
    {
        private readonly WeakReferenceCollection<EventEntry> _entrys = new WeakReferenceCollection<EventEntry>();
        
        private void EventAction([NotNull] string topic, [NotNull] Action<EventEntry> entryAction, ErrorTracer errorTracer)
        {
            errorTracer.Phase = "Resolve Topic " + Argument.NotNull(topic, nameof(topic));

            var entry = _entrys.FirstOrDefault(ent => ent.Topic == topic);
            var add = false;
            if (entry == null)
            {
                add = true;
                entry = new EventEntry(topic);
            }

            Argument.NotNull(entryAction, nameof(entryAction))(entry);

            if (add) _entrys.Add(entry);
        }
        
        private class EventEntry : IWeakReference
        {
            private static readonly MethodInfo Handlermethod = typeof(EventEntry).GetMethod(
                "GeneralHandler",
                BindingFlags.Instance |
                BindingFlags.NonPublic);
            
            public EventEntry([NotNull] string topic) => Topic = Argument.NotNull(topic, nameof(topic));

            [UsedImplicitly(ImplicitUseKindFlags.Access)]
            private void GeneralHandler(object sender, EventArgs args)
            {
                foreach (var weakDelegate in _handler)
                    weakDelegate.Invoke(sender, args);
            }
            
            private class HandlerEntry : IWeakReference
            {
                [UsedImplicitly]
                private readonly Delegate _dDelegate;
                
                public bool IsAlive => _delegate.IsAlive;
                
                public void Invoke([NotNull] object sender, [NotNull] object args)
                {
                    Argument.NotNull(args, nameof(args));
                    Argument.NotNull(sender, nameof(sender));

                    switch (_type)
                    {
                        case InvokeType.Zero:
                            _delegate.Invoke();
                            break;
                        case InvokeType.One:
                            _delegate.Invoke(args);
                            break;
                        case InvokeType.Two:
                            _delegate.Invoke(sender, args);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                private enum InvokeType
                {
                    Zero = 0,
                    One = 1,
                    Two = 2
                }

                private readonly WeakDelegate _delegate;
                private readonly InvokeType _type;
                
                public HandlerEntry([NotNull] Delegate dDelegate)
                {
                    _dDelegate = Argument.NotNull(dDelegate, nameof(dDelegate));
                    _type = (InvokeType) dDelegate.Method.GetParameters().Length;
                    _delegate = new WeakDelegate(dDelegate);
                }
                
                public HandlerEntry([NotNull] MethodInfo methodInfo, [NotNull] object target)
                {
                    _type = (InvokeType) Argument.NotNull(methodInfo, nameof(methodInfo)).GetParameters().Length;
                    _delegate = new WeakDelegate(methodInfo, Argument.NotNull(target, nameof(target)));
                }
            }
            
            private readonly WeakReferenceCollection<HandlerEntry> _handler =
                new WeakReferenceCollection<HandlerEntry>();
            
            private readonly WeakCollection<object> _publisher = new WeakCollection<object>();
            
            public string Topic { get; }
            
            public bool IsAlive => _handler.Count != 0 && _publisher.Count != 0;
            
            public void AddPublisher([NotNull] EventInfo info, [NotNull] object publisher, [NotNull] ErrorTracer errorTracer)
            {
                Argument.NotNull(errorTracer, nameof(errorTracer)).Phase = "Adding Publisher " + Topic;
                Argument.NotNull(info, nameof(info)).AddEventHandler(Argument.NotNull(publisher, nameof(publisher)), Delegate.CreateDelegate(info.EventHandlerType, this, Handlermethod));
                _publisher.Add(publisher);
            }
            
            public void Addhandler([NotNull] Delegate dDelegate, [NotNull] ErrorTracer errorTracer)
            {
                Argument.NotNull(errorTracer, nameof(errorTracer)).Phase = "Adding Handler " + Topic;
                _handler.Add(new HandlerEntry(dDelegate));
            }
            
            public void Addhandler([NotNull] MethodInfo dDelegate, [NotNull] object target, [NotNull] ErrorTracer errorTracer)
            {
                Argument.NotNull(errorTracer, nameof(errorTracer)).Phase = "Adding Handler " + Topic;
                _handler.Add(new HandlerEntry(dDelegate, target));
            }
            
        }
        
        public void AddEventHandler([NotNull] string topic, [NotNull] Delegate handler, [NotNull] ErrorTracer errorTracer)
        {
            Argument.NotNull(errorTracer, nameof(errorTracer));
            Argument.NotNull(handler, nameof(handler));

            lock (this)
                EventAction(Argument.NotNull(topic, nameof(topic)), ev => ev.Addhandler(handler, errorTracer), errorTracer);
        }
        
        public void AddEventHandler(string topic, MethodInfo handler, object target, ErrorTracer errorTracer) => EventAction(topic, entry => entry.Addhandler(handler, target, errorTracer), errorTracer);

        public void AddPublisher(string topic, EventInfo eventInfo, object publisher, ErrorTracer errorTracer)
        {
            lock (this)
                EventAction(topic, ev => ev.AddPublisher(eventInfo, publisher, errorTracer), errorTracer);
        }
    }
}