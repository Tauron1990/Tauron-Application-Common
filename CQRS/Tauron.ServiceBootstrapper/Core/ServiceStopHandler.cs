using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tauron.Application.CQRS.Client;
using Tauron.Application.CQRS.Client.Commands;
using Tauron.Application.CQRS.Client.Events;
using Tauron.Application.CQRS.Client.Infrastructure;
using Tauron.Application.CQRS.Common.Configuration;
using Tauron.Application.CQRS.Extensions.ServiceControl;

namespace Tauron.ServiceBootstrapper.Core
{
    [CQRSHandler]
    public sealed class ServiceStopHandler : ICommandHandler<StopServiceCommand>
    {
        private readonly IOptions<ClientCofiguration> _options;
        private readonly ILogger<ServiceStopHandler> _logger;
        private readonly IEventPublisher _eventPublisher;

        public ServiceStopHandler(IOptions<ClientCofiguration> options, ILogger<ServiceStopHandler> logger, IEventPublisher eventPublisher)
        {
            _options = options;
            _logger = logger;
            _eventPublisher = eventPublisher;
        }

        public async Task<OperationResult> Handle(StopServiceCommand message)
        {
            if (message.Name != _options.Value.ServiceName) return OperationResult.Success;
            
            _logger.LogInformation("Shutdown Service");
            await _eventPublisher.Publish(new ServiceStoppedEvent(message.Name));
            await BootStrapper.Shutdown();

            return OperationResult.Success;
        }
    }
}