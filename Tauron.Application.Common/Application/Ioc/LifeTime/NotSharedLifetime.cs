namespace Tauron.Application.Ioc.LifeTime
{
    public sealed class NotSharedLifetime : ILifetimeContext
    {
        public bool IsAlive => false;

        public object GetValue() => null;

        public void SetValue(object value)
        {
        }
    }
}