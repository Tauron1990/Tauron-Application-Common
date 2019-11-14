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
        private readonly IEventManager _eventManager;

        public EventHub(IConnectionManager manager, IEventManager eventManager)
        {
            _manager = manager;
            _eventManager = eventManager;
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await _manager.Disconected(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        [HubMethodName(HubMethodNames.Subscribe), UsedImplicitly]
        public async Task Subscribe(string[] events)
        {
            await _manager.CheckId(Context.ConnectionId);
            await _manager.AddSubscription(Context.ConnectionId, events);
        }

        [HubMethodName(HubMethodNames.SendingSuccseded), UsedImplicitly]
        public async Task SendingSuccseded(long id)
        {
            await _manager.CheckId(Context.ConnectionId);
            await _manager.SendingOk(id, Context.ConnectionId);
        }

        [HubMethodName(HubMethodNames.PublishEvent), UsedImplicitly]
        public async Task PublishEvent(DomainMessage domainMessage)
        {
            await _manager.CheckId(Context.ConnectionId);
            domainMessage.Sender = Context.ConnectionId;
            await _eventManager.DeliverEvent(domainMessage);
        }

        [HubMethodName(HubMethodNames.PublishEventGroup), UsedImplicitly]
        public async Task StoreEvents(DomainMessage[] events)
        {
            await _manager.CheckId(Context.ConnectionId);
            foreach (var domainMessage in events) 
                domainMessage.Sender = Context.ConnectionId;
            await _eventManager.StoreEvents(events);
        }

        [HubMethodName(HubMethodNames.PublishEventToClient), UsedImplicitly]
        public async Task PublishEventToClient(DomainMessage message, string client)
        {
            await _manager.CheckId(Context.ConnectionId);
            await Clients.Client(client).SendAsync(HubMethodNames.PropagateEvent, message);
        }

        [HubMethodName(HubMethodNames.HeartbeatNames.StillConnected), UsedImplicitlyAttribute]
        public async Task StillConnected()
            => await _manager.StillConnected(Context.ConnectionId);
    }
}