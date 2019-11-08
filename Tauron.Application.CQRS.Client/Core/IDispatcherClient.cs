using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tauron.Application.CQRS.Client.Commands;
using Tauron.Application.CQRS.Client.Events;
using Tauron.Application.CQRS.Client.Infrastructure;
using Tauron.Application.CQRS.Client.Querys;
using Tauron.Application.CQRS.Common.Server;

namespace Tauron.Application.CQRS.Client.Core
{
    public delegate Task MessageHandler(IMessage? message, DomainMessage domainMessage);

    public interface IDispatcherClient
    {
        Task Connect();

        Task Stop();

        Task Subscribe(IEnumerable<(string Name, MessageHandler)> intrests);

        Task<OperationResult> SendCommand(ICommand command);

        Task SendEvent(IAmbientEvent ambientEvent);

        Task StoreEvents(IEnumerable<IEvent> events);

        Task<TResponse?> Query<TQuery, TResponse>(IQueryHelper<TResponse> query) 
            where TResponse : class, IQueryResult;

        Task SendToClient(string client, IMessage message);

        Task SendToClient(string client, OperationResult operationResult, long operationId);

        Task Start();
    }
}