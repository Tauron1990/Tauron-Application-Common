using System.Threading.Tasks;

namespace Tauron.Application.CQRS.Client.Events.Invoker
{
    public interface IEventInvokerBase
    {
        Task Handle(IEvent @event);
    }
}