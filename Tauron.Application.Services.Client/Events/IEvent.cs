using System;
using Tauron.Application.Services.Client.Infrastructure;

namespace Tauron.Application.Services.Client.Events
{
    public interface IEvent : IAmbientEvent
    {
        Guid Id { get; set; }

        long Version { get; set; }

        DateTimeOffset TimeStamp { get; set; }
    }
}