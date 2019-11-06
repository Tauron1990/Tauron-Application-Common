using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tauron.Application.CQRS.Client.Commands;
using Tauron.Application.CQRS.Client.Events;
using Tauron.Application.CQRS.Client.Infrastructure;
using Tauron.Application.CQRS.Client.Querys;
using Tauron.Application.CQRS.Common;
using Tauron.Application.CQRS.Common.Configuration;
using Tauron.Application.CQRS.Common.Server;

namespace Tauron.Application.CQRS.Client.Core.Components
{
    public sealed class DispatcherClient : ICoreDispatcherClient, IDisposable
    {
        private class MessageDelivery
        {
            private readonly DomainMessage _message;
            private readonly EventRegistration _eventRegistration;

            public MessageDelivery(DomainMessage message, EventRegistration eventRegistration)
            {
                _message = message;
                _eventRegistration = eventRegistration;
            }

            public Task Execute() => _eventRegistration.Execute(_message);
        }

        private class EventRegistration
        {
            private readonly List<MessageHandler> _handlers;

            public EventRegistration(IEnumerable<MessageHandler> handlers) 
                => _handlers = new List<MessageHandler>(handlers);

            public void RegisterHandler(MessageHandler handler) 
                => _handlers.Add(handler);

            public async Task Execute(DomainMessage domainMessage)
            {
                foreach (var messageHandler in _handlers) 
                    await messageHandler(domainMessage.ToRealMessage(), domainMessage);
            }
        }

        private readonly ConcurrentDictionary<string, EventRegistration> _eventRegistrations = new ConcurrentDictionary<string, EventRegistration>();
        private readonly MessageQueue<MessageDelivery> _messageQueue;
        private readonly SignalRConnectionManager _connectionManager;

        public DispatcherClient(IOptions<ClientCofiguration> configuration, ILogger<ICoreDispatcherClient> logger, IDispatcherApi api, IErrorManager errorManager)
        {
            _connectionManager = new SignalRConnectionManager(configuration, logger, errorManager, api);
            _connectionManager.MessageRecived += message =>
            {
                if (_eventRegistrations.TryGetValue(message.EventName, out var eventRegistration)) 
                    _messageQueue.Enqueue(new MessageDelivery(message, eventRegistration));
            };

            _messageQueue = new MessageQueue<MessageDelivery>();
            _messageQueue.OnError += exception =>
            {
                logger.LogError(exception, "Error on Delivering Message");
                return Task.CompletedTask;
            };
            _messageQueue.OnWork += delivery => delivery.Execute();
        }

        public async Task Connect() => await _connectionManager.Connect();

        public async Task Stop() => await _connectionManager.Disconnect();

        public async Task Subscribe(IEnumerable<(string Name, MessageHandler)> intrests)
        {
            var temp = intrests.GroupBy(g => g.Name).ToArray();
            foreach (var intrest in temp) 
                _eventRegistrations[intrest.Key] = new EventRegistration(intrest.Select(g => g.Item2));

            await _connectionManager.Call(HubMethodNames.Subscribe, new object[] { temp.Select(g => g.Key).ToArray() });
        }

        public async Task<OperationResult> SendCommand(ICommand command)
        {
            
        }

        public async Task SendEvent(IAmbientEvent ambientEvent) => throw new NotImplementedException();

        public async Task StoreEvents(IEnumerable<IEvent> events) => throw new NotImplementedException();

        public async Task<TResponse> Query<TQuery, TResponse>(IQueryHelper<TResponse> query) where TResponse : IQueryResult => throw new NotImplementedException();

        public async Task SendToClient(string client, IMessage serverDomainMessage) => throw new NotImplementedException();

        public async Task SendToClient(string client, OperationResult serverDomainMessage) => throw new NotImplementedException();

        public Task Start() 
            => _messageQueue.Start();

        public void AddHandler(string name, MessageHandler handler)
        {
            if (_eventRegistrations.TryGetValue(name, out var registration)) 
                registration.RegisterHandler(handler);
        }

        public void Dispose() 
            => _messageQueue.Dispose();
    }
}