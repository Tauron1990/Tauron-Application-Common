using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace Tauron.Application.CQRS.Client
{
    public interface IConnectionStadeManager
    {
        public event Func<string, Task>? ConnectionFailedEvent;

        Task ConnectionFailed(string message);

        public HubConnectionState HubConnectionState { get; set; }
    }
}