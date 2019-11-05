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

        public int Version { get; set; }

        public DateTimeOffset TimeStamp { get; set; }
    }
}