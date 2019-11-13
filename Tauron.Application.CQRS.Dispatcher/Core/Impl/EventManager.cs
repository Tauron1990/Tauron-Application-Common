using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tauron.Application.CQRS.Common;
using Tauron.Application.CQRS.Common.Server;
using Tauron.Application.CQRS.Dispatcher.EventStore.Data;

namespace Tauron.Application.CQRS.Dispatcher.Core.Impl
{
    public class EventManager : IEventManager
    {
        private readonly MessageQueue<DomainMessage> _messageQueue = new MessageQueue<DomainMessage>();
        private readonly IObjectFactory _objectFactory;

        public EventManager(IObjectFactory objectFactory, ILogger<IEventManager> logger, IConnectionManager connectionManager)
        {
            _objectFactory = objectFactory;

            _messageQueue.OnError += exception =>
            {
                logger.LogError(exception, "Error On Processing Message");
                return Task.CompletedTask;
            };
            _messageQueue.OnWork += async message => await connectionManager.SendEvent(message);
        }

        public async Task StoreEvents(DomainMessage[] domainMessages)
        {
            using var scope = _objectFactory.CreateDatabase();
            var database = scope.Target;

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

        public async Task StartMessageQueue() 
            => await _messageQueue.Start();

        public void StopMessageQueue() 
            => _messageQueue.Stop();
    }
}
