using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Tauron.Application.CQRS.Common;
using Tauron.Application.CQRS.Common.Configuration;
using Tauron.Application.CQRS.Common.Dto;
using Tauron.Application.CQRS.Common.Server;

namespace Tauron.Application.CQRS.Client.Core.Components
{
    public sealed class SignalRConnectionManager
    {
        public event Action<DomainMessage>? MessageRecived;

        private readonly IOptions<ClientCofiguration> _configuration;
        private readonly ILogger<IDispatcherClient> _logger;
        private readonly IErrorManager _errorManager;
        private readonly IDispatcherApi _dispatcherApi;
        private readonly HubConnection _connection;

        private string _oldId = string.Empty;

        public SignalRConnectionManager(IOptions<ClientCofiguration> configuration, ILogger<IDispatcherClient> logger, IErrorManager errorManager,
            IDispatcherApi dispatcherApi)
        {
            _configuration = configuration;
            _logger = logger;
            _errorManager = errorManager;
            _dispatcherApi = dispatcherApi;
            _connection = new HubConnectionBuilder().WithUrl(configuration.Value.EventHubUrl).AddJsonProtocol().Build();
            _connection.Closed += ConnectionOnClosed;

            _connection.On(HubMethodNames.PropagateEvent, new Func<DomainMessage, long, Task>(EventRecived));
            _connection.On(HubMethodNames.HeartbeatNames.Heartbeat, async () => await _connection.SendAsync(HubMethodNames.HeartbeatNames.StillConnected));
        }

        private async Task EventRecived(DomainMessage msg, long id)
        {
            await new SynchronizationContextRemover();
            try
            {
                MessageRecived?.Invoke(msg);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error on Processing Message");
            }
            await _connection.SendAsync(HubMethodNames.SendingSuccseded, id);
        }

        private async Task ConnectionOnClosed(Exception arg)
        {
            if (arg == null) return;

            await Task.Delay(3000);
            await Connect();
        }

        public async Task<bool> Connect()
        {
            try
            {
                await _connection.StartAsync();
                await Validate();

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error on Connect to Server");
                await _errorManager.ConnectionFailed(e.Message);

                return false;
            }
        }

        public Task Call(string name, params object[] args) 
            => _connection.SendCoreAsync(name, args);

        public Task Call(string name, Array arg)
            => _connection.SendAsync(name, arg);

        public async Task Disconnect() 
            => await _connection.StopAsync();

        private async Task Validate()
        {
            if(!await _dispatcherApi.Validate(new ValidateConnection(_connection.ConnectionId, _oldId, _configuration.Value.ApiKey)))
                throw new InvalidOperationException("Validation Failed");
            _oldId = _connection.ConnectionId;
        }
    }
}