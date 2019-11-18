using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Tauron.Application.CQRS.Common;
using Tauron.Application.CQRS.Common.Dto;
using Tauron.Application.CQRS.Common.Server;
using Tauron.Application.CQRS.Dispatcher.EventStore.Data;
using Tauron.Application.CQRS.Dispatcher.Hubs;

namespace Tauron.Application.CQRS.Dispatcher.Core.Impl
{
    public class ConnectionManager : IConnectionManager, IDisposable
    {
        private class Registration
        {
            public string Id { get; set; }

            public string? ServiceName { get; set; }

            public bool Validated { get; set; }

            public string[] EventSubscriptions { get; set; }

            public bool Disconnected { get; set; }

            public Stopwatch LastHeartBeat { get; }

            public Registration(string id, KnowenService service)
            {
                LastHeartBeat = Stopwatch.StartNew();
                Id = id;

                ServiceName = service.Name;
                EventSubscriptions = service.Subscriptions;
            }
        }

        private class PendingMessage
        {
            public string ServiceName { get; }

            public DomainMessage DomainMessage { get; }

            public DateTimeOffset Timeout { get; }

            public PendingMessage(string serviceName, DomainMessage domainMessage, DateTimeOffset timeout)
            {
                ServiceName = serviceName;
                DomainMessage = domainMessage;
                Timeout = timeout;
            }
        }

        private readonly ConcurrentDictionary<string, List<PendingMessage>> _pendingMessages = new ConcurrentDictionary<string, List<PendingMessage>>();
        private readonly ConcurrentDictionary<string, Registration> _registrations = new ConcurrentDictionary<string, Registration>();
        private readonly ConcurrentDictionary<string, CirculationList> _circulationLists = new ConcurrentDictionary<string, CirculationList>();

        private readonly ConcurrentBag<PendingMessage> _added = new ConcurrentBag<PendingMessage>();
        private readonly ConcurrentBag<PendingMessage> _removed = new ConcurrentBag<PendingMessage>();

        private readonly ReaderWriterLock _databaseLock = new ReaderWriterLock();
        private readonly Timer _messageTimer;
        private readonly TimeSpan _messageTimeout;

        private readonly IObjectFactory _objectFactory;
        private readonly HubLifetimeManager<EventHub> _lifetimeManager;
        private readonly IHubContext<EventHub> _hubContext;
        private readonly ILogger<IConnectionManager> _logger;

        private readonly IServiceRegistrationStore _store;

        public ConnectionManager(IObjectFactory objectFactory, HubLifetimeManager<EventHub> lifetimeManager, IHubContext<EventHub> hubContext, ILogger<IConnectionManager> logger, 
            IConfiguration configuration, IServiceRegistrationStore store)
        {
            try
            {
                var timeout = configuration.GetValue<int>("MessageTimeout");
                _messageTimeout = TimeSpan.FromSeconds(timeout);
            }
            catch 
            {
                _messageTimeout = TimeSpan.FromSeconds(30);
            }

            _objectFactory = objectFactory;
            _lifetimeManager = lifetimeManager;
            _hubContext = hubContext;
            _logger = logger;
            _store = store;
            _messageTimer = new Timer(CheckMessages);
            using var context = objectFactory.CreateDatabase();

            foreach (var pendingMessage in context.Target.PendingMessages.ToArray().GroupBy(pm => pm.ServiceName))
            {
                _pendingMessages.AddOrUpdate(pendingMessage.Key,
                    s => new List<PendingMessage>(pendingMessage.Select(m => new PendingMessage(s, m.ToDomainMessage(), m.Timeout))),
                    (s, list) =>
                    {
                        lock (list) 
                            list.AddRange(pendingMessage.Select(m => new PendingMessage(s, m.ToDomainMessage(), m.Timeout)));

                        return list;
                    });
            }

            foreach (var knowenService in _store.GetAllServices()) 
                _registrations[knowenService.Name] = new Registration(string.Empty, knowenService) {Disconnected = true};

            _messageTimer.Change(TimeSpan.FromSeconds(30), Timeout.InfiniteTimeSpan);
        }

        private async void CheckMessages(object? state)
        {
            _databaseLock.AcquireReaderLock(TimeSpan.FromMinutes(2));
            try
            {
                using var scope = _objectFactory.CreateDatabase();
                await using var context = scope.Target;
                var dbmessages = context.PendingMessages.ToArray().GroupBy(pm => pm.ServiceName).ToArray();

                var dbDic = new Dictionary<string, List<PendingMessageEntity>>();

                foreach (var group in dbmessages)
                    dbDic[group.Key] = new List<PendingMessageEntity>(group);

                var cookie = _databaseLock.UpgradeToWriterLock(TimeSpan.FromMinutes(1));

                try
                {
                    var currentTime = DateTimeOffset.Now;

                    foreach (var (_, pendingMessages) in _pendingMessages)
                    {
                        lock (pendingMessages)
                        {
                            foreach (var pendingMessage in pendingMessages.Where(pendingMessage => currentTime > pendingMessage.Timeout))
                            {
                                _removed.Add(pendingMessage);
                                pendingMessages.Remove(pendingMessage);
                            }
                        }
                    }

                    var removeGroups = _removed.GroupBy(m => m.ServiceName).ToArray();
                    var addedGroups = _added.GroupBy(m => m.ServiceName).ToArray();

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

                    _removed.Clear();
                    _added.Clear();

                    await context.SaveChangesAsync();
                }
                finally
                {
                    _databaseLock.DowngradeFromWriterLock(ref cookie);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error on Updating Pending Messages");
            }
            finally
            {
                _databaseLock.ReleaseReaderLock();
                _messageTimer.Change(TimeSpan.FromSeconds(30), Timeout.InfiniteTimeSpan);
            }

        }

        public int GetCurrentClients() 
            => _registrations.Count;

        public int GetPendingMessages() 
            => _pendingMessages.Count;

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
                        Guard.CheckNull(registration.ServiceName),
                        // ReSharper disable once InconsistentlySynchronizedField
                        s => new List<PendingMessage>(new[] {new PendingMessage(s, domainMessage, DateTimeOffset.Now + _messageTimeout)}),
                        (s, list) =>
                        {
                            lock (list)
                                list.Add(new PendingMessage(s, domainMessage, DateTimeOffset.Now + _messageTimeout));

                            return list;
                        });
                }

                IClientProxy toSend;

                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (domainMessage.EventType)
                {
                    case EventType.Command:
                    case EventType.Query:
                        var list = _circulationLists.AddOrUpdate(Guard.CheckNull(domainMessage.EventName), s => new CirculationList(GetSubscripedClients(s)), 
                            (s, circulationList) => circulationList.Replace(GetSubscripedClients(s)));

                        var nextId = list.GetNext();
                        var firstId = nextId;
                        if (nextId == null) return;

                        while (true)
                        {
                            if (_registrations.TryGetValue(Guard.CheckNull(nextId), out var registration) && !registration.Disconnected)
                                break;

                            var tempId = list.GetNext();

                            if (tempId == nextId || firstId == tempId)
                            {
                                var msg = new DomainMessage
                                {
                                    EventData = JsonSerializer.Serialize(OperationResult.Failed(OperationError.Error(-111, "Service Offline")), typeof(OperationResult)),
                                    TypeName = typeof(OperationResult).AssemblyQualifiedName,
                                    EventName = typeof(OperationResult).Name,
                                    EventType = EventType.Command,
                                    OperationId = domainMessage.OperationId
                                };

                                if (domainMessage.EventType == EventType.Command)
                                    await _hubContext.Clients.Client(domainMessage.Sender).SendAsync(HubMethodNames.PropagateEvent, msg, msg.OperationId);

                                return;
                            }
                            else
                                nextId = tempId;
                        }

                        toSend = _hubContext.Clients.Client(nextId);

                        break;
                    default:
                        toSend = _hubContext.Clients.Groups(domainMessage.EventName);
                        break;
                }

                await toSend.SendAsync(HubMethodNames.PropagateEvent, domainMessage, domainMessage.OperationId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error on Sending Event");
            }
            finally
            {
                _databaseLock.ReleaseReaderLock();
            }
        }

        public async Task Validated(string id, string serviceName, string oldId)
        {
            if (_registrations.TryGetValue(oldId, out var registration))
            {
                registration.Disconnected = false;
                registration.Id = id;

                _registrations[id] = registration;
                _registrations.TryRemove(oldId, out _);

                foreach (var (eventName, list) in _circulationLists) 
                    list.Replace(GetSubscripedClients(eventName));
            }
            else
                _registrations[id] = new Registration(id, await _store.Get(serviceName))
                {
                    Validated = true, 
                    ServiceName = serviceName
                };
        }

        public Task Disconected(string id)
        {
            foreach (var registration in _registrations.Where(r => r.Value.Id == id))
                registration.Value.Disconnected = false;

            return Task.CompletedTask;
        }

        public async Task AddSubscription(string id, string[] events)
        {
            try
            {
                if (_registrations.TryGetValue(_registrations.First(p => p.Value.Id == id).Key, out var registration))
                {
                    registration.EventSubscriptions = events;
                    if(registration.ServiceName != null)
                        await _store.UpdateSubscriptions(registration.ServiceName, events);
                }

                foreach (var @event in events)
                {
                    await _lifetimeManager.AddToGroupAsync(id, @event);
                    if(_circulationLists.TryGetValue(@event, out var  list))
                        list.Replace(GetSubscripedClients(@event));

                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error on Updating Subscriptions");
            }
        }

        public Task SendingOk(long eventId, string connectionId)
        {
            _databaseLock.AcquireReaderLock(TimeSpan.FromSeconds(30));

            try
            {
                if (_pendingMessages.TryGetValue(_registrations.First(r => r.Value.Id == connectionId).Key, out var list))
                {
                    lock (list)
                    {
                        var msg = list.FirstOrDefault(pm => pm.DomainMessage.OperationId == eventId);
                        if (msg != null)
                        {
                            list.Remove(msg);
                            _removed.Add(msg);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error on Sending OK Update");
            }
            finally
            {
                _databaseLock.ReleaseReaderLock();
            }

            return Task.CompletedTask;
        }

        public Task UpdateAllConnection()
        {
            foreach (var registration in _registrations.Select(r => r.Value))
            {
                if (registration.LastHeartBeat.Elapsed.Seconds > 60)
                    registration.Disconnected = true;
            }

            return Task.CompletedTask;
        }

        public Task StillConnected(string id)
        {
            var reg = _registrations.FirstOrDefault(r => r.Value.Id == id).Value;
            if (reg == null) return Task.CompletedTask;

            reg.Disconnected = false;
            reg.LastHeartBeat.Restart();

            return Task.CompletedTask;
        }

        public void Dispose() 
            => _messageTimer.Dispose();

        private IEnumerable<string> GetSubscripedClients(string eventName) 
            => _registrations.Select(r => r.Value).Where(r => r.EventSubscriptions.Contains(eventName) && !r.Disconnected).Select(r => r.Id);
    }
}
