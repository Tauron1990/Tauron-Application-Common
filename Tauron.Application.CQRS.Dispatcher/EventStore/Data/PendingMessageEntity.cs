using System;
using System.ComponentModel.DataAnnotations;
using Tauron.Application.CQRS.Common.Server;

namespace Tauron.Application.CQRS.Dispatcher.EventStore.Data
{
    public class PendingMessageEntity
    {
        public string? ServiceName { get; set; }
        
        [Key]
        public long SequenceNumber { get; set; }

        public Guid? Id { get; set; }

        public EventType EventType { get; set; }

        public string? Data { get; set; }

        public string? EventName { get; set; }

        public string? OriginType { get; set; }

        public long? Version { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        public long OperationId { get; set; }

        public DateTimeOffset Timeout { get; set; }

        public DomainMessage ToDomainMessage()
        {
            return new DomainMessage
            {
                OperationId = OperationId,
                Id = Id,
                EventType = EventType,
                EventData = Data,
                EventName = EventName,
                TypeName = OriginType,
                Version = Version,
                TimeStamp = TimeStamp,
            };
        }

        public PendingMessageEntity()
        {
            
        }

        public PendingMessageEntity(DomainMessage msg, string serviceName)
        {
            ServiceName = serviceName;
            Id = msg.Id;
            EventType = msg.EventType;
            Data = msg.EventData;
            EventName = msg.EventName;
            OriginType = msg.TypeName;
            Version = msg.Version;
            TimeStamp = msg.TimeStamp;
            OperationId = msg.OperationId;
        }
    }
}