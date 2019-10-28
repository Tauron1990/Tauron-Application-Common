using System.Threading.Tasks;
using Tauron.Application.CQRS.Client.Infrastructure;

namespace Tauron.Application.CQRS.Client.Events
{
    public interface IEventExecutor<in TEvent>
    where TEvent : class, IMessage
    {
        Task Apply(TEvent eEvent);
    }
}