using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Tauron.Application.CQRS.Dispatcher.Core.Impl
{
    public class JsonServiceRegistratonStore : IServiceRegistrationStore
    {
        private readonly ILogger<IServiceRegistrationStore> _logger;

        private readonly string _savePath;
        private readonly ConcurrentDictionary<string, KnowenService> _services = new ConcurrentDictionary<string, KnowenService>();

        public JsonServiceRegistratonStore(ILogger<IServiceRegistrationStore> logger)
        {
            _logger = logger;
            var dic = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Tauron", "CQRS-Dispatcher");
            if (!Directory.Exists(dic))
                Directory.CreateDirectory(dic);

            _savePath = Path.Combine(dic, "Services.xml");

            Read();
        }

        public IEnumerable<KnowenService> GetAllServices() => _services.Values;

        public Task<KnowenService> Get(string serviceName)
            => Task.FromResult(_services.GetOrAdd(serviceName, s => new KnowenService(s, new string[0])));

        public async Task UpdateSubscriptions(string serviceName, string[] events)
        {
            (await Get(serviceName)).SafeExcange(events);
            await Save();
        }

        private void Read()
        {
            try
            {
                _services.Clear();
                var dic = JsonSerializer.Deserialize<List<KnowenService>>(File.ReadAllText(_savePath));

                foreach (var knowenService in dic) 
                    _services[knowenService.Name] = knowenService;
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Error on Read Service Registration Store");
                _services.Clear();
            }
        }

        private async Task Save()
        {
            try
            {
                var toSave = new List<KnowenService>(_services.Values);
                var data = JsonSerializer.Serialize(toSave);
                await File.WriteAllTextAsync(_savePath, data);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error on Save Services");
            }
        }
    }
}