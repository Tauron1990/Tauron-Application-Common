using System;

namespace Tauron.Application.Ioc.LifeTime
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class CacheCreationProcessAttribute : Attribute
    {
        public bool Cache { get; }

        public CacheCreationProcessAttribute(bool cache) => Cache = cache;
    }
}