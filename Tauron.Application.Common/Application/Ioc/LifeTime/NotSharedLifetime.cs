namespace Tauron.Application.Ioc.LifeTime
{
    [CacheCreationProcess(true)]
    public sealed class NotSharedLifetime : ILifetimeContext
    {
        public bool IsAlive => false;

        public object GetValue() => null;

        public void SetValue(object value)
        {
        }
    }
}