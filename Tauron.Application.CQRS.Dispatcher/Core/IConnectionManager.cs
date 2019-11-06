using System.Threading.Tasks;

namespace Tauron.Application.CQRS.Dispatcher.Core
{
    public interface IConnectionManager
    {
        Task Validated(string id, string serviceName, string oldId);

        Task Disconected(string id);

        Task AddSubscription(string id, string[] events);

        Task SendingOk(int eventId, string connectionId);
    }
}