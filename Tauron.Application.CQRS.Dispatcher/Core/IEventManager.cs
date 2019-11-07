using System.Threading.Tasks;
using Tauron.Application.CQRS.Common.Server;

namespace Tauron.Application.CQRS.Dispatcher.Core
{
    public interface IEventManager
    {
        Task StoreEvents(DomainMessage[] domainMessages);

        Task DeliverEvent(DomainMessage message);
    }
}