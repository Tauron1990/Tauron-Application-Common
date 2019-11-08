using System.Threading.Tasks;
using Tauron.Application.CQRS.Common.Server;

namespace Tauron.Application.CQRS.Dispatcher.Core
{
    public interface IConnectionManager
    {
        Task CheckId(string id);

        Task SendEvent(DomainMessage domainMessage);

        Task Validated(string id, string serviceName, string oldId);

        Task Disconected(string id);

        Task AddSubscription(string id, string[] events);

        Task SendingOk(int eventId, string connectionId);
    }
}