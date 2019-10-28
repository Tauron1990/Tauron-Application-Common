using System.Threading.Tasks;

namespace Tauron.Application.CQRS.Client.Events
{
    public interface IEventPublisher
    {
        Task Publish<TEvent>(TEvent @event)
            where TEvent : IAmbientEvent;
    }
}