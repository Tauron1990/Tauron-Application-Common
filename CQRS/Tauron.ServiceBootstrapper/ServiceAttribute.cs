using System;
using Microsoft.Extensions.DependencyInjection;

namespace Tauron.ServiceBootstrapper
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ServiceAttribute : Attribute
    {
        public ServiceLifetime Lifetime { get; }

        public Type Interface { get; set; }

        public ServiceAttribute(ServiceLifetime lifetime) 
            => Lifetime = lifetime;
    }
}