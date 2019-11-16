using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace Tauron.Application.CQRS.Client.Core
{
    public class ConnectionStadeManager : IConnectionStadeManager
    {
        public event Func<string, Task>? ConnectionFailedEvent; 

        public Task ConnectionFailed(string message) 
            => ConnectionFailedEvent?.Invoke(message) ?? Task.CompletedTask;

        public HubConnectionState HubConnectionState { get; set; }
    }
}