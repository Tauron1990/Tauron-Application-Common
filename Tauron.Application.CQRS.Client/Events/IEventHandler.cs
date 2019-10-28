using System.Threading.Tasks;

namespace Tauron.Application.CQRS.Client.Events
{
    public interface IEventHandler<in TEvent>
        where TEvent : IAmbientEvent
    {
        Task Handle(TEvent @Event);
    }
}