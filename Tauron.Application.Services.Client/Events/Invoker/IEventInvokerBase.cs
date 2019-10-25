using System.Threading.Tasks;

namespace Tauron.Application.Services.Client.Events.Invoker
{
    public interface IEventInvokerBase
    {
        Task Handle(IEvent @event);
    }
}