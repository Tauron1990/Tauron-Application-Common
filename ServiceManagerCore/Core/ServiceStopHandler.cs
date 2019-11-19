using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tauron.Application.CQRS.Client;
using Tauron.Application.CQRS.Client.Events;
using Tauron.Application.CQRS.Extensions.ServiceControl;

namespace ServiceManager.Core.Core
{
    [CQRSHandler]
    public class ServiceStopHandler : IEventHandler<ServiceStoppedEvent>
    {
        public static event Func<ServiceStoppedEvent, Task>? ServiceStoppedEvent;

        private readonly ILogger<ServiceStopHandler> _logger;

        public ServiceStopHandler(ILogger<ServiceStopHandler> logger) => _logger = logger;

        public async Task Handle(ServiceStoppedEvent message)
        {
            try
            {
                var invoker = ServiceStoppedEvent;
                if(invoker == null) return;

                await invoker(message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error on handling Service Stopped");
            }
        }
    }
}