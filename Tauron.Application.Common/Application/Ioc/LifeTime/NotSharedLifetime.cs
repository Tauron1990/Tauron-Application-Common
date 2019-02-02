namespace Tauron.Application.Ioc.LifeTime
{
    public sealed class NotSharedLifetime : ILifetimeContext
    {
        public static readonly NotSharedLifetime DefaultNotSharedLifetime = new NotSharedLifetime();

        public bool IsAlive => false;

        public object GetValue() => null;

        public void SetValue(object value)
        {
        }
    }
}