﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Tauron.Application.CQRS.Common;
using Tauron.Application.CQRS.Common.Server;
using Tauron.Application.CQRS.Dispatcher.Hubs;

namespace Tauron.Application.CQRS.Dispatcher.Core.Impl
{
    public class ConnectionManager : IConnectionManager
    {
        private class Registration
        {
            public string Id { get; set; }

            public bool Validated { get; set; }
        }

        private readonly ConcurrentDictionary<string, Registration> _registrations = new ConcurrentDictionary<string, Registration>();
        private readonly HubLifetimeManager<EventHub> _lifetimeManager;
        private readonly IHubContext<EventHub> _hubContext;

        public ConnectionManager(HubLifetimeManager<EventHub> lifetimeManager, IHubContext<EventHub> hubContext)
        {
            _lifetimeManager = lifetimeManager;
            _hubContext = hubContext;
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
    }
}
