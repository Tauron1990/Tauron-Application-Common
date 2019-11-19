using System.Threading.Tasks;
using Tauron.Application.CQRS.Client;
using Tauron.Application.CQRS.Client.Events;
using Tauron.Application.CQRS.Extensions.ServiceControl.Logging;

namespace ServiceManager.Core.Core
{
    [CQRSHandler]
    public class LogReciever : IEventHandler<LoggingEvent>
    {
        private readonly LogEntries _logEntries;

        public LogReciever(LogEntries logEntries) 
            => _logEntries = logEntries;

        public Task Handle(LoggingEvent message)
        {
            _logEntries.AddLog(message.ServiceName, message.Message);

            return Task.CompletedTask;
        }
    }
}