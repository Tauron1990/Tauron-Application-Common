using System;
using JetBrains.Annotations;
using Tauron.Application.Ioc.LifeTime;

namespace Tauron.Application.Ioc
{
    [AttributeUsage(AttributeTargets.Class)]
    [PublicAPI]
    public sealed class ThreadSharedAttribute : LifetimeContextAttribute
    {
        public ThreadSharedAttribute()
            : base(typeof(ThreadSharedLifetime))
        {
        }
    }
}