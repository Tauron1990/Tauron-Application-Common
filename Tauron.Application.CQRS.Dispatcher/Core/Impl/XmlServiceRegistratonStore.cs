using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Tauron.Application.CQRS.Dispatcher.Core.Impl
{
    public class XmlServiceRegistratonStore : IServiceRegistrationStore
    {
        private static readonly XmlSerializer _xmlSerializer = new XmlSerializer(typeof(KnowenService));
        
        private readonly 
        private readonly ConcurrentDictionary<string, KnowenService> _services = new ConcurrentDictionary<string, KnowenService>();

        public XmlServiceRegistratonStore() 
            => Read();

        public IEnumerable<KnowenService> GetAllServices() => _services.Values;

        public Task<KnowenService> Get(string serviceName)
            => Task.FromResult(_services.GetOrAdd(serviceName, s => new KnowenService(s, new string[0])));

        public async Task UpdateSubscriptions(string serviceName, string[] events)
            => (await Get(serviceName)).SafeExcange(events);

        private void Read()
        {

        }


    }
}