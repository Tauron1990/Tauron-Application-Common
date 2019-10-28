using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tauron.Application.CQRS.Client.Commands;
using Tauron.Application.CQRS.Client.Events;
using Tauron.Application.CQRS.Client.Infrastructure;
using Tauron.Application.CQRS.Client.Querys;
using Tauron.Application.CQRS.Common.Server;

namespace Tauron.Application.CQRS.Client.Core.Components
{
    public sealed class DispatcherClient : ICoreDispatcherClient
    {
        public async Task Connect() => throw new NotImplementedException();

        public async Task Stop() => throw new NotImplementedException();

        public async Task Subscribe(IEnumerable<(string Name, Func<IMessage, DomainMessage, Task> Handler)> intrests) => throw new NotImplementedException();

        public async Task<OperationResult> SendCommand(ICommand command) => throw new NotImplementedException();

        public async Task SendEvent(IAmbientEvent ambientEvent) => throw new NotImplementedException();

        public async Task StoreEvents(IEnumerable<IEvent> events) => throw new NotImplementedException();

        public async Task<TResponse> Query<TQuery, TResponse>(IQueryHelper<TResponse> query) where TResponse : IQueryResult => throw new NotImplementedException();

        public async Task SendToClient(string client, IMessage serverDomainMessage) => throw new NotImplementedException();

        public async Task SendToClient(string client, OperationResult serverDomainMessage) => throw new NotImplementedException();

        public void AddHandler(string name, Func<IMessage, DomainMessage, Task> handler)
        {
            throw new NotImplementedException();
        }
    }
}