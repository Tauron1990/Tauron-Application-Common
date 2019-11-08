using System;

namespace Tauron.Application.CQRS.Common.Server
{
    public class DomainMessage
    {
        public long OperationId { get; set; }

        public string? EventName { get; set; }

        public string? EventData { get; set; }

        public EventType EventType { get; set; }
        
        public long? Version { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        public Guid? Id { get; set; }

        public string? TypeName { get; set; }

        public string? Sender { get; set; }
    }
}