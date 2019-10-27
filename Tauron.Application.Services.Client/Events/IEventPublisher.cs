using System.Threading.Tasks;

namespace Tauron.Application.Services.Client.Events
{
    public interface IEventPublisher
    {
        Task Publish<TEvent>(TEvent @event)
            where TEvent : IAmbientEvent;
    }
}