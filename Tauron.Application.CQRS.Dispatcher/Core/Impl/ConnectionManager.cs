using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Tauron.Application.CQRS.Common;
using Tauron.Application.CQRS.Common.Server;
using Tauron.Application.CQRS.Dispatcher.EventStore;
using Tauron.Application.CQRS.Dispatcher.EventStore.Data;
using Tauron.Application.CQRS.Dispatcher.Hubs;

namespace Tauron.Application.CQRS.Dispatcher.Core.Impl
{
    public class ConnectionManager : IConnectionManager, IDisposable
    {
        private class Registration
        {
            public string Id { get; set; }

            public string ServiceName { get; set; }

            public bool Validated { get; set; }

            public string[] EventSubscriptions { get; set; } = new string[0];

            public Registration(string id)
            {
                Id = id;
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

        private readonly ConcurrentBag<PendingMessage> _added = new ConcurrentBag<PendingMessage>();
        private readonly ConcurrentBag<PendingMessage> _removed = new ConcurrentBag<PendingMessage>();

        private readonly ReaderWriterLock _databaseLock = new ReaderWriterLock();
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

            foreach (var pendingMessage in context.PendingMessages.GroupBy(pm => pm.ServiceName))
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
            _databaseLock.AcquireReaderLock(TimeSpan.FromMinutes(2));
            try
            {
                using var scope = _objectFactory.CreateDatabase();
                await using var context = scope.Target;
                var dbmessages = context.PendingMessages.GroupBy(pm => pm.ServiceName).ToArray();

                var dbDic = new Dictionary<string, List<PendingMessageEntity>>();

                foreach (var group in dbmessages)
                    dbDic[group.Key] = new List<PendingMessageEntity>(group);

                var cookie = _databaseLock.UpgradeToWriterLock(TimeSpan.FromMinutes(1));

                try
                {
                    var removeGroups = _removed.GroupBy(m => m.ServiceName).ToArray();
                    var addedGroups = _added.GroupBy(m => m.ServiceName).ToArray();

                    foreach (var removeGroup in removeGroups)
                    {
                        if (!dbDic.TryGetValue(removeGroup.Key, out var list)) continue;
                        
                        foreach (var pendingMessage in removeGroup)
                        {
                            var ent = list.FirstOrDefault(pme => pme.OperationId == pendingMessage.DomainMessage.OperationId);
                            if (ent != null)
                                context.Remove(ent);
                        }
                    }

                    foreach (var addedGroup in addedGroups)
                    {
                        if (!dbDic.TryGetValue(addedGroup.Key, out var list)) continue;

                        foreach (var pendingMessage in addedGroup)
                        {
                            var ent = list.FirstOrDefault(pme => pme.OperationId == pendingMessage.DomainMessage.OperationId);
                            if (ent == null)
                                context.Add(new PendingMessageEntity(pendingMessage.DomainMessage, pendingMessage.ServiceName));
                        }
                    }

                    _removed.Clear();
                    _added.Clear();

                    await context.SaveChangesAsync();
                }
                finally
                {
                    _databaseLock.DowngradeFromWriterLock(ref cookie);
                }
            }
            finally
            {
                _databaseLock.ReleaseReaderLock();
                _messageTimer.Change(TimeSpan.FromSeconds(30), Timeout.InfiniteTimeSpan);
            }

        }

        public Task CheckId(string id)
        {
            if (_registrations.TryGetValue(id, out var registration) && registration.Validated) return Task.CompletedTask;

            return Task.FromException(new HubException("Id Nicht Validiert"));
        }

        public async Task SendEvent(DomainMessage domainMessage)
        {
            _databaseLock.AcquireReaderLock(TimeSpan.FromSeconds(30));

            try
            {
                var registrations = _registrations.Where(r => r.Value.EventSubscriptions.Contains(domainMessage.EventName)).Select(r => r.Value).ToArray();

                foreach (var registration in registrations)
                {
                    _pendingMessages.AddOrUpdate(
                        registration.ServiceName,
                        s => new List<PendingMessage>(new[] {new PendingMessage(s, domainMessage),}),
                        (s, list) =>
                        {
                            lock (list) 
                                list.Add(new PendingMessage(s, domainMessage));

                            return list;
                        });
                }

                await _hubContext.Clients.Groups(domainMessage.EventName).SendAsync(HubMethodNames.PropagateEvent, domainMessage);
            }
            finally
            {
                _databaseLock.ReleaseReaderLock();
            }
        }

        public Task Validated(string id, string serviceName, string oldId)
        {
            _registrations.AddOrUpdate(
                serviceName,
                s => new Registration(id) { Validated = true },
                (s, registration) =>
                {
                    registration.Id = id;
                    registration.Validated = true;

                    return registration;
                });

            return Task.CompletedTask;
        }

        public Task Disconected(string id)
        {
            foreach (var registration in _registrations.Where(r => r.Value.Id == id)) 
                _registrations.Remove(registration.Key, out _);

            return Task.CompletedTask;
        }

        public async Task AddSubscription(string id, string[] events)
        {
            if (_registrations.TryGetValue(_registrations.First(p => p.Value.Id == id).Key, out var registration)) 
                registration.EventSubscriptions = events;

            foreach (var @event in events) 
                await _lifetimeManager.AddToGroupAsync(id, @event);
        }

        public Task SendingOk(int eventId, string connectionId)
        {
            _databaseLock.AcquireReaderLock(TimeSpan.FromSeconds(30));

            try
            {
                if (_pendingMessages.TryGetValue(_registrations.First(r => r.Value.Id == connectionId).Key, out var list))
                {
                    lock (list)
                    {
                        
                    }
                }
            }
            finally
            {
                _databaseLock.ReleaseReaderLock();
            }
        }

        public void Dispose() 
            => _messageTimer.Dispose();
    }
}
