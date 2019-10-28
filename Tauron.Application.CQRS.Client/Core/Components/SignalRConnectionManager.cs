using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using Tauron.Application.CQRS.Common.Configuration;

namespace Tauron.Application.CQRS.Client.Core.Components
{
    public sealed class SignalRConnectionManager
    {
        public HubConnection Connection { get; private set; }

        public SignalRConnectionManager(IOptions<ClientCofiguration> configuration)
        {
            var builder = new HubConnectionBuilder();
        }
    }
}