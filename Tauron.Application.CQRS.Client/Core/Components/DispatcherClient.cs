using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tauron.Application.CQRS.Client.Commands;
using Tauron.Application.CQRS.Client.Core.Components.Handler;
using Tauron.Application.CQRS.Client.Events;
using Tauron.Application.CQRS.Client.Infrastructure;
using Tauron.Application.CQRS.Client.Querys;
using Tauron.Application.CQRS.Common;
using Tauron.Application.CQRS.Common.Configuration;
using Tauron.Application.CQRS.Common.Server;

namespace Tauron.Application.CQRS.Client.Core.Components
{
    //TODO AutoReconnection with teimer
    //TODO Make Conntent State Visible

    [UsedImplicitly]
    public sealed class DispatcherClient : ICoreDispatcherClient, IDisposable
    {
        private class OperationWaiter : IDisposable
        {
            private readonly ManualResetEventSlim _manualResetEvent;
            private OperationResult? _result;

            public OperationWaiter() 
                => _manualResetEvent = new ManualResetEventSlim(false);

            public OperationResult Wait()
            {
                _manualResetEvent.Wait(TimeSpan.FromSeconds(30));
                return _result ?? OperationResult.Failed(OperationError.Error(-1, "Zeitüberschreitung der Anforderung. Der Server Antortet nicht oder es wurde Keine antwort geschiekt."));
            }

            public void Push(OperationResult? result)
            {
                _result = result;
                _manualResetEvent.Set();
            }

            public void Dispose() => _manualResetEvent.Dispose();
        }

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
                foreach (var messageHandler in _handlers.ToArray()) 
                    await messageHandler(domainMessage.ToRealMessage(), domainMessage);
            }
        }

        private readonly IServiceScopeFactory _scopeFactory;

        private readonly ConcurrentDictionary<long, OperationWaiter> _operationWaiters = new ConcurrentDictionary<long, OperationWaiter>();
        private readonly ConcurrentDictionary<string, EventRegistration> _eventRegistrations = new ConcurrentDictionary<string, EventRegistration>();

        private readonly MessageQueue<MessageDelivery> _messageQueue;
        private readonly SignalRConnectionManager _connectionManager;

        public DispatcherClient(IOptions<ClientCofiguration> configuration, ILogger<ICoreDispatcherClient> logger, IDispatcherApi api, IErrorManager errorManager,
            IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            _connectionManager = new SignalRConnectionManager(configuration, logger, errorManager, api);
            _connectionManager.MessageRecived += message =>
            {
                if (message.EventType == EventType.CommandResult)
                {
                    if(_operationWaiters.TryGetValue(message.OperationId, out var waiter))
                        waiter.Push(message.ToRealMessage<OperationResult>());
                }
                else
                {
                    if (_eventRegistrations.TryGetValue(message.EventName ?? string.Empty, out var eventRegistration))
                        _messageQueue.Enqueue(new MessageDelivery(message, eventRegistration));
                }
            };

            _messageQueue = new MessageQueue<MessageDelivery>();
            _messageQueue.OnError += exception =>
            {
                logger.LogError(exception, "Error on Delivering Message");
                return Task.CompletedTask;
            };
            _messageQueue.OnWork += delivery => delivery.Execute();
        }

        public async Task Connect()
        {
            if (!await _connectionManager.Connect())
            {
                throw new InvalidOperationException("Error on Connect Server");
            }
        }

        public async Task Stop()
        {
            await _connectionManager.Disconnect();
            _messageQueue.Stop();
        }

        public async Task Subscribe(IEnumerable<(string Name, MessageHandler)> intrests)
        {
            var temp = intrests.GroupBy(g => g.Name).ToArray();
            foreach (var intrest in temp) 
                _eventRegistrations[intrest.Key] = new EventRegistration(intrest.Select(g => g.Item2));

            await _connectionManager.Call(HubMethodNames.Subscribe, (Array)temp.Select(g => g.Key).ToArray());
        }

        public async Task<OperationResult> SendCommand(ICommand command)
        {
            var msg = command.ToDomainMessage();
            using var waiter = new OperationWaiter();

            try
            {
                _operationWaiters[msg.OperationId] = waiter;
                await _connectionManager.Call(HubMethodNames.PublishEvent, msg);

                return waiter.Wait();
            }
            finally
            {
                _operationWaiters.TryRemove(msg.OperationId, out _);
            }
        }

        public Task SendEvent(IAmbientEvent ambientEvent) 
            => _connectionManager.Call(HubMethodNames.PublishEvent, ambientEvent);

        public Task StoreEvents(IEnumerable<IEvent> events) 
            => _connectionManager.Call(HubMethodNames.PublishEventGroup, (Array) events.Select(e => e.ToDomainMessage()).ToArray());

        public async Task<TResponse?> Query<TQuery, TResponse>(IQueryHelper<TResponse> query) where TResponse : class, IQueryResult
        {
            using var scope = _scopeFactory.CreateScope();
            using var awaiter = scope.ServiceProvider.GetRequiredService<QueryAwaiter<TResponse>>();

            var eventName = typeof(TResponse).Name;

            if (!_eventRegistrations.ContainsKey(eventName)) 
                _eventRegistrations[eventName] = new EventRegistration(new MessageHandler[] {new HandlerListDelegator(new List<Func<HandlerBase>>(), _scopeFactory).Handle});

            return await awaiter.SendQuery(query, msg => _connectionManager.Call(HubMethodNames.PublishEvent, msg.ToDomainMessage()));
        }

        public async Task SendToClient(string client, IMessage message) 
            => await _connectionManager.Call(HubMethodNames.PublishEventToClient, message.ToDomainMessage(), client);

        public async Task SendToClient(string client, OperationResult operationResult, long operationId)
        {
            var msg = operationResult.ToDomainMessage();
            msg.OperationId = operationId;

            await _connectionManager.Call(HubMethodNames.PublishEventToClient, msg, client);
        }

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