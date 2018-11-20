using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Tauron.Application.Ioc;

namespace Tauron.Application
{
    [PublicAPI]
    public abstract class SharedEvent<TPayload>
    {
        private readonly WeakActionEvent<TPayload> _handlerList = new WeakActionEvent<TPayload>();
        
        public virtual void Publish(TPayload content) => _handlerList.Invoke(content);

        public void Subscribe([NotNull] Action<TPayload> handler) => _handlerList.Add(Argument.NotNull(handler, nameof(handler)));

        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Un")]
        public void UnSubscribe([NotNull] Action<TPayload> handler) => _handlerList.Remove(Argument.NotNull(handler, nameof(handler)));
        
    }

    [PublicAPI]
    public interface IEventAggregator
    {
        [NotNull]
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        TEventType GetEvent<TEventType, TPayload>() where TEventType : SharedEvent<TPayload>, new();
    }

    [Export(typeof(IEventAggregator))]
    [PublicAPI]
    public sealed class EventAggregator : IEventAggregator
    {
        private static IEventAggregator _aggregator;
        
        private readonly Dictionary<Type, object> _events = new Dictionary<Type, object>();
        
        [NotNull]
        public static IEventAggregator Aggregator
        {
            get
            {
                if (_aggregator != null) return _aggregator;

                _aggregator =
                    (IEventAggregator) CommonApplication.Current.Container.Resolve(typeof(IEventAggregator), null);
                return _aggregator;
            }

            set
            {
                if (_aggregator == null)
                    _aggregator = value;
            }
        }
        
        public TEventType GetEvent<TEventType, TPayload>() where TEventType : SharedEvent<TPayload>, new()
        {
            var t = typeof(TEventType);
            if (!_events.ContainsKey(t)) _events[t] = new TEventType();

            return (TEventType) _events[t];
        }
    }
}