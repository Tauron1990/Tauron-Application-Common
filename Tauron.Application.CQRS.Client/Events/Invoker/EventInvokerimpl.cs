using System.Threading.Tasks;

namespace Tauron.Application.CQRS.Client.Events.Invoker
{
    public sealed class EventInvokerImpl<TType> : IEventInvoker<TType> where TType : class, IEvent
    {
        private readonly IEventExecutor<TType> _eventExecutor;

        public EventInvokerImpl(IEventExecutor<TType> eventExecutor) 
            => _eventExecutor = eventExecutor;

        public Task Handle(TType @event) 
            => _eventExecutor.Apply(@event);

        public Task Handle(IEvent @event) 
            => Handle((TType) @event);
    }
}