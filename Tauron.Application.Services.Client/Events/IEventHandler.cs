using System.Threading.Tasks;

namespace Tauron.Application.Services.Client.Events
{
    public interface IEventHandler<in TEvent>
        where TEvent : IAmbientEvent
    {
        Task Handle(TEvent @Event);
    }
}