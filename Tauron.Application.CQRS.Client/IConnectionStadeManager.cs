using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace Tauron.Application.CQRS.Client
{
    public interface IConnectionStadeManager
    {
        event Func<string, Task>? ConnectionFailedEvent;

        Task ConnectionFailed(string message);

        HubConnectionState HubConnectionState { get; set; }
    }
}