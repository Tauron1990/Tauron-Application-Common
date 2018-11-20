using System;
using JetBrains.Annotations;

namespace Tauron.Application.Ioc.LifeTime
{
    [PublicAPI]
    public sealed class SharedLifetime : MarshalByRefObject, ILifetimeContext
    {
        private object _value;

        public bool IsAlive => _value != null;

        public object GetValue() => _value;

        public void SetValue(object value) => _value = value;
    }
}