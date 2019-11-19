using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tauron.Application.CQRS.Dispatcher.Core
{
    public interface IServiceRegistrationStore
    {
        IEnumerable<KnowenService> GetAllServices();

        Task<KnowenService> Get(string serviceName);

        Task UpdateSubscriptions(string serviceName, string[] events);

        Task<bool> Remove(string name);
    }
}