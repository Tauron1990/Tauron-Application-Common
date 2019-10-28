using System;

namespace Tauron.Application.CQRS.Client.Events
{
    public interface IEvent : IAmbientEvent
    {
        Guid Id { get; set; }

        long Version { get; set; }

        DateTimeOffset TimeStamp { get; set; }
    }
}