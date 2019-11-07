using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.SignalR;
using Tauron.Application.CQRS.Common;
using Tauron.Application.CQRS.Common.Server;
using Tauron.Application.CQRS.Dispatcher.Core;

namespace Tauron.Application.CQRS.Dispatcher.Hubs
{
    public sealed class EventHub : Hub
    {
        private readonly IConnectionManager _manager;

        public EventHub(IConnectionManager manager) 
            => _manager = manager;

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await _manager.Disconected(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        [HubMethodName(HubMethodNames.Subscribe), UsedImplicitly]
        public async Task Subscribe(string[] events) => await _manager.AddSubscription(Context.ConnectionId, events);

        [HubMethodName(HubMethodNames.SendingSuccseded), UsedImplicitly]
        public async Task SendingSuccseded(int id) 
            => await _manager.SendingOk(id, Context.ConnectionId);

        [HubMethodName(HubMethodNames.PublishEvent), UsedImplicitly]
        public async Task PublishEvent(DomainMessage domainMessage)
        {

        }

        [HubMethodName(HubMethodNames.PublishEventGroup), UsedImplicitly]
        public async Task StoreEvents(DomainMessage[] events)
        {

        }

        [HubMethodName(HubMethodNames.PublishEventToClient), UsedImplicitly]
        public async Task PublishEventToVlient(DomainMessage message, string client)
        {

        }
    }
}