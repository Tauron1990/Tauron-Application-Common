using System;

namespace Tauron.Application.Services.Client.Events
{
    public abstract class EventBase : IEvent
    {
        public Guid Id { get; set; }

        public int Version { get; set; }

        internal void SetIdAndVersion(Guid id, int version)
        {
            id = Id;
            Version = version;
        }
    }
}