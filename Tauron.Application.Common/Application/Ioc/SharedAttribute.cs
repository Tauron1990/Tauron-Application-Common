using System;
using JetBrains.Annotations;
using Tauron.Application.Ioc.LifeTime;

namespace Tauron.Application.Ioc
{
    [AttributeUsage(AttributeTargets.Class)]
    [PublicAPI]
    public sealed class SharedAttribute : LifetimeContextAttribute
    {
        public SharedAttribute()
            : base(typeof(SharedLifetime))
        {
        }
    }
}