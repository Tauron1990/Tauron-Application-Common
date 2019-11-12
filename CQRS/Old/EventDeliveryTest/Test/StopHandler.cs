using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Tauron.Application.CQRS.Client;
using Tauron.Application.CQRS.Client.Commands;
using Tauron.Application.CQRS.Client.Events;
using Tauron.Application.CQRS.Client.Infrastructure;
using Tauron.Application.CQRS.Extensions.ServiceControl;

namespace EventDeliveryTest.Test
{
    [CQRSHandler]
    public class StopHandler : ICommandHandler<StopServiceCommand>
    {
        private readonly IEventPublisher _publisher;

        public StopHandler(IEventPublisher publisher) => _publisher = publisher;

        public async Task<OperationResult> Handle(StopServiceCommand message)
        {
            await _publisher.Publish(new ServiceStoppedEvent("Temp"));

            Console.WriteLine("Delay Close");

            await Task.Delay(1_000);

            Process.GetCurrentProcess().Kill();

            return OperationResult.Success;
        }
    }
}