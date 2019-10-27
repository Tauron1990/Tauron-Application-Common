using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tauron.Application.CQRS.Common.Server;
using Tauron.Application.Services.Client.Commands;
using Tauron.Application.Services.Client.Events;
using Tauron.Application.Services.Client.Infrastructure;
using Tauron.Application.Services.Client.Querys;
using Tauron.CQRS.Services.Core;

namespace Tauron.Application.Services.Client.Core.Components
{
    public sealed class DispatcherClient : ICoreDispatcherClient
    {
        public async Task Connect() => throw new NotImplementedException();

        public async Task Stop() => throw new NotImplementedException();

        public async Task Subscribe(IEnumerable<(string Name, Func<IMessage, DomainMessage, Task> Handler)> intrests) => throw new NotImplementedException();

        public async Task<OperationResult> SendCommand(ICommand command) => throw new NotImplementedException();

        public async Task SendEvent(IAmbientEvent ambientEvent) => throw new NotImplementedException();

        public async Task StoreEvents(IEnumerable<IEvent> events) => throw new NotImplementedException();

        public async Task<TResponse> Query<TQuery, TResponse>(IQueryHelper<TResponse> query) where TResponse : IMessage => throw new NotImplementedException();

        public async Task SendToClient(string client, DomainMessage serverDomainMessage) => throw new NotImplementedException();

        public async Task SendToClient(string client, OperationResult serverDomainMessage) => throw new NotImplementedException();

        public void AddHandler(string name, Func<IMessage, DomainMessage, Task> handler)
        {
            throw new NotImplementedException();
        }
    }
}