using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tauron.Application.CQRS.Common.Server;
using Tauron.Application.Services.Client.Commands;
using Tauron.Application.Services.Client.Events;
using Tauron.Application.Services.Client.Infrastructure;
using Tauron.Application.Services.Client.Querys;

namespace Tauron.Application.Services.Client.Core
{
    public interface IDispatcherClient
    {
        Task Connect();

        Task Stop();

        Task Subscribe(IEnumerable<(string Name, Func<IMessage, DomainMessage, Task> Handler)> intrests);

        Task<OperationResult> SendCommand(ICommand command);

        Task SendEvent(IAmbientEvent ambientEvent);

        Task StoreEvents(IEnumerable<IEvent> events);

        Task<TResponse> Query<TQuery, TResponse>(IQueryHelper<TResponse> query) 
            where TResponse : IMessage;

        Task SendToClient(string client, DomainMessage serverDomainMessage);

        Task SendToClient(string client, OperationResult serverDomainMessage);
    }
}