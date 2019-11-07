using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tauron.Application.CQRS.Common;
using Tauron.Application.CQRS.Common.Server;
using Tauron.Application.CQRS.Dispatcher.EventStore;
using Tauron.Application.CQRS.Dispatcher.EventStore.Data;

namespace Tauron.Application.CQRS.Dispatcher.Core.Impl
{
    public class EventManager : IEventManager
    {
        private readonly MessageQueue<DomainMessage> _messageQueue = new MessageQueue<DomainMessage>();
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConnectionManager _connectionManager;

        public EventManager(IServiceScopeFactory scopeFactory, ILogger<IEventManager> logger, IConnectionManager connectionManager)
        {
            _scopeFactory = scopeFactory;
            _connectionManager = connectionManager;

            _messageQueue.OnError += exception =>
            {
                logger.LogError(exception, "Error On Processing Message");
                return Task.CompletedTask;
            };
            _messageQueue.OnWork += async message => await _connectionManager.SendEvent(message);
        }

        public async Task StoreEvents(DomainMessage[] domainMessages)
        {
            using var scope = _scopeFactory.CreateScope();
            var database = scope.ServiceProvider.GetRequiredService<DispatcherDatabaseContext>();

            await database.AddRangeAsync(domainMessages.Select(dm => new EventEntity(dm)));
            await database.SaveChangesAsync();
            foreach (var domainMessage in domainMessages) 
                _messageQueue.Enqueue(domainMessage);
        }

        public Task DeliverEvent(DomainMessage message)
        {
            _messageQueue.Enqueue(message);
            return Task.CompletedTask;
        }
    }
}
