using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Tauron.Application.CQRS.Common;
using Tauron.Application.CQRS.Common.Server;
using Tauron.Application.CQRS.Dispatcher.EventStore;
using Tauron.Application.CQRS.Dispatcher.Hubs;

namespace Tauron.Application.CQRS.Dispatcher.Core.Impl
{
    public class ConnectionManager : IConnectionManager, IDisposable
    {
        private class Registration
        {
            public string Id { get; }

            public bool Validated { get; }

            public Registration(string id, bool validated)
            {
                Id = id;
                Validated = validated;
            }
        }

        private class PendingMessage
        {
            public string ServiceName { get; }

            public DomainMessage DomainMessage { get; }

            public Stopwatch Stopwatch { get; set; }

            public PendingMessage(string serviceName, DomainMessage domainMessage)
            {
                Stopwatch = Stopwatch.StartNew();
                ServiceName = serviceName;
                DomainMessage = domainMessage;
            }
        }

        private readonly ConcurrentDictionary<string, List<PendingMessage>> _pendingMessages = new ConcurrentDictionary<string, List<PendingMessage>>();
        private readonly ConcurrentDictionary<string, Registration> _registrations = new ConcurrentDictionary<string, Registration>();

        private readonly Timer _messageTimer;

        private readonly IObjectFactory _objectFactory;
        private readonly HubLifetimeManager<EventHub> _lifetimeManager;
        private readonly IHubContext<EventHub> _hubContext;

        public ConnectionManager(IObjectFactory objectFactory, HubLifetimeManager<EventHub> lifetimeManager, IHubContext<EventHub> hubContext,
            DispatcherDatabaseContext context)
        {
            _objectFactory = objectFactory;
            _lifetimeManager = lifetimeManager;
            _hubContext = hubContext;
            _messageTimer = new Timer(CheckMessages);

            foreach (var pendingMessage in context.PendingMessages.GroupBy(pm => pm.ServicaName))
            {
                _pendingMessages.AddOrUpdate(pendingMessage.Key,
                    s => new List<PendingMessage>(pendingMessage.Select(m => new PendingMessage(s, m.ToDomainMessage()))),
                    (s, list) =>
                    {
                        lock (list) 
                            list.AddRange(pendingMessage.Select(m => new PendingMessage(s, m.ToDomainMessage())));

                        return list;
                    });
            }

            _messageTimer.Change(TimeSpan.FromSeconds(30), Timeout.InfiniteTimeSpan);
        }

        private async void CheckMessages(object? state)
        {
            try
            {
                using var scope = _objectFactory.CreateDatabase();
                await using var context = scope.Target;
                var dbmessages = context.PendingMessages.ToArray();


            }
            finally
            {
                _messageTimer.Change(TimeSpan.FromSeconds(30), Timeout.InfiniteTimeSpan);
            }

        }

        public Task CheckId(string id)
        {
            if (_registrations.TryGetValue(id, out var registration) && registration.Validated) return Task.CompletedTask;

            return Task.FromException(new HubException("Id Nicht Validiert"));
        }

        public async Task SendEvent(DomainMessage domainMessage) 
            => await _hubContext.Clients.Groups(domainMessage.EventName).SendAsync(HubMethodNames.PropagateEvent, domainMessage);

        public Task Validated(string id, string serviceName, string oldId)
        {
            throw new NotImplementedException();
        }

        public Task Disconected(string id)
        {
            throw new NotImplementedException();
        }

        public Task AddSubscription(string id, string[] events)
        {
            throw new NotImplementedException();
        }

        public Task SendingOk(int eventId, string connectionId)
        {
            throw new NotImplementedException();
        }

        public void Dispose() 
            => _messageTimer.Dispose();
    }
}
