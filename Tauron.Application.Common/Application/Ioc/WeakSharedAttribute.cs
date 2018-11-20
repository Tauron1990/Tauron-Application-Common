using System;
using JetBrains.Annotations;
using Tauron.Application.Ioc.LifeTime;

namespace Tauron.Application.Ioc
{
    [AttributeUsage(AttributeTargets.Class)]
    [PublicAPI]
    public sealed class WeakSharedAttribute : LifetimeContextAttribute
    {
        public WeakSharedAttribute()
            : base(typeof(WeakSharedLifetime))
        {
        }
    }
}