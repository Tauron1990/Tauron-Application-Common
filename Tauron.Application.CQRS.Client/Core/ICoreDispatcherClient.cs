using System;
using System.Threading.Tasks;
using Tauron.Application.CQRS.Common.Server;

namespace Tauron.Application.CQRS.Client.Core
{
    public interface ICoreDispatcherClient : IDispatcherClient
    {
        void AddHandler(string name, MessageHandler handler);
    }
}