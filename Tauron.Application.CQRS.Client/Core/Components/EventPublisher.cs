using System.Threading.Tasks;
using Tauron.Application.CQRS.Client.Events;

namespace Tauron.Application.CQRS.Client.Core.Components
{
    public class EventPublisher : IEventPublisher
    {
        private readonly IDispatcherClient _dispatcher;

        public EventPublisher(IDispatcherClient dispatcher) => _dispatcher = dispatcher;

        public Task Publish<T>(T @event) 
            where T : IAmbientEvent => _dispatcher.SendEvent(@event);
    }
}