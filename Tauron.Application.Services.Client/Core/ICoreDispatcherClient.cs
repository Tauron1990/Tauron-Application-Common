using System;
using System.Threading.Tasks;
using Tauron.Application.CQRS.Common.Server;
using Tauron.Application.Services.Client.Core;
using Tauron.Application.Services.Client.Infrastructure;

namespace Tauron.CQRS.Services.Core
{
    public interface ICoreDispatcherClient : IDispatcherClient
    {
        void AddHandler(string name, Func<IMessage, DomainMessage, Task> handler);
    }
}