using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tauron.Application.CQRS.Dispatcher.Core.Impl
{
    public class InMemoryServiceRegistratonStore : IServiceRegistrationStore
    {
        private readonly ConcurrentDictionary<string, KnowenService> _services = new ConcurrentDictionary<string, KnowenService>();

        public IEnumerable<KnowenService> GetAllServices() 
            => _services.Values;

        public Task<KnowenService> Get(string serviceName) 
            => Task.FromResult(_services.GetOrAdd(serviceName, s => new KnowenService(s, new string[0])));

        public async Task UpdateSubscriptions(string serviceName, string[] events) 
            => (await Get(serviceName)).SafeExcange(events);

        public Task<bool> Remove(string name)
        {
            _services.Remove(name, out _);

            return Task.FromResult(true);
        }
    }
}