using Tauron.Application.CQRS.Client.Commands;

namespace Tauron.Application.CQRS.Extensions.ServiceControl
{
    public class StopServiceCommand : IAmbientCommand
    {
        public string? Name { get; set; }
    }
}
