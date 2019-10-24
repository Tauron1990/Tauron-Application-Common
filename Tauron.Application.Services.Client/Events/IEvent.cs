using System;
using Tauron.Application.Services.Client.Core;

namespace Tauron.Application.Services.Client.Events
{
    public interface IEvent : IMessage
    {
        Guid Id { get; set; }

        int Version { get; set; }
    }
}