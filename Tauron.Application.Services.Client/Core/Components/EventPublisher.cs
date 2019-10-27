using System.Threading.Tasks;
using Tauron.Application.Services.Client.Events;

namespace Tauron.Application.Services.Client.Core.Components
{
    public class EventPublisher : IEventPublisher
    {
        private readonly IDispatcherClient _dispatcher;

        public EventPublisher(IDispatcherClient dispatcher) => _dispatcher = dispatcher;

        public Task Publish<T>(T @event) 
            where T : IAmbientEvent => _dispatcher.SendEvent(@event);
    }
}