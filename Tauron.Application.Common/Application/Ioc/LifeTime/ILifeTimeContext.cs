using JetBrains.Annotations;

namespace Tauron.Application.Ioc.LifeTime
{
    [PublicAPI]
    public interface ILifetimeContext
    {
        bool IsAlive { get; }

        object GetValue();

        void SetValue(object value);
    }
}