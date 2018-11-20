using System;

namespace Tauron.Application.Ioc.LifeTime
{
    public sealed class WeakSharedLifetime : ILifetimeContext
    {
        private WeakReference _reference;

        public bool IsAlive => _reference != null && _reference.IsAlive;

        public object GetValue() => _reference.Target;

        public void SetValue(object value) => _reference = new WeakReference(value);
    }
}