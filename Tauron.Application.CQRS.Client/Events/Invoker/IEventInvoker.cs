using System.Threading.Tasks;

namespace Tauron.Application.CQRS.Client.Events.Invoker
{
    public interface IEventInvoker<in TEvent> : IEventInvokerBase
        where TEvent : class, IEvent
    {
        Task Handle(TEvent @event);
    }
}