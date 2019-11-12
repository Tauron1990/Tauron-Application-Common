using System;
using Tauron.Application.CQRS.Client.Core;
using Tauron.Application.CQRS.Client.Events;

namespace Tauron.Application.CQRS.Extensions.ServiceControl
{
    public class ServiceStoppedEvent : EventBase
    {
        private static readonly Guid Namespace = Guid.Parse("F26B69D6-B39A-4E9E-B445-064305D118C1");

        public string ServiceName { get; }

        public ServiceStoppedEvent(string serviceName) 
            : base(IdGenerator.Generator.NewGuid(Namespace, serviceName), 0)
        {
            ServiceName = serviceName;
        }
    }
}