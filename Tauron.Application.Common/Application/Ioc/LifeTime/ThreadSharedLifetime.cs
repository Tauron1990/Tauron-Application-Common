using System.Collections.Generic;
using System.Threading;

namespace Tauron.Application.Ioc.LifeTime
{
    [CacheCreationProcess(true)]
    public sealed class ThreadSharedLifetime : ILifetimeContext
    {
        private readonly Dictionary<Thread, object> _objects = new Dictionary<Thread, object>();

        public bool IsAlive => _objects.ContainsKey(Thread.CurrentThread);

        public object GetValue() => _objects.TryGetValue(Thread.CurrentThread, out var value) ? value : null;

        public void SetValue(object value) => _objects[Thread.CurrentThread] = value;
    }
}