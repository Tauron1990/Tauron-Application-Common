using System;
using System.ComponentModel.DataAnnotations;
using Tauron.Application.CQRS.Common.Server;

namespace Tauron.Application.CQRS.Dispatcher.EventStore.Data
{
    public class EventEntity
    {
        [Key]
        public long SequenceNumber { get; set; }

        public Guid? Id { get; set; }

        public EventType EventType { get; set; }

        public string Data { get; set; }

        public string EventName { get; set; }

        public string OriginType { get; set; }

        public long? Version { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        public EventEntity()
        {
            
        }

        public EventEntity(DomainMessage domainMessage)
        {
            Id = domainMessage.Id;
            EventType = domainMessage.EventType;
            Data = domainMessage.EventData;
            EventName = domainMessage.EventName;
            OriginType = domainMessage.TypeName;
            Version = domainMessage.Version;
            TimeStamp = domainMessage.TimeStamp;
        }

        public DomainMessage ToDomainMessage()
        {
            return new DomainMessage
            {
                OperationId = SequenceNumber,
                Id = Id,
                EventType = EventType,
                EventData = Data,
                EventName = EventName,
                TypeName = OriginType,
                Version = Version,
                TimeStamp = TimeStamp
            };
        }
    }
}