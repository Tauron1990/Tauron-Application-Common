using System;

namespace Tauron.Application.Services.Client.Events
{
    public abstract class EventBase : IEvent
    {
        public Guid Id { get; set; }

        public long Version { get; set; }

        public DateTimeOffset TimeStamp { get; set; } = DateTimeOffset.UtcNow;
    }
}