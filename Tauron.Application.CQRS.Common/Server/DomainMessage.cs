using System;

namespace Tauron.Application.CQRS.Common.Server
{
    public class DomainMessage
    {
        public long? SequenceNumber { get; set; }

        public string EventName { get; set; } = string.Empty;

        public string EventData { get; set; } = string.Empty;

        public EventType? EventType { get; set; }
        
        public long? Version { get; set; }

        public DateTimeOffset? TimeStamp { get; set; }

        public Guid? Id { get; set; }

        public string TypeName { get; set; } = string.Empty;

        public string Sender { get; set; }
    }
}