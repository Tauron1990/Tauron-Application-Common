using JetBrains.Annotations;
using Tauron.Application.Ioc.LifeTime;

namespace Tauron.Application.Common.CastleProxy.Aop.Threading
{
    public abstract class ThreadingBaseAspect : AspectBaseAttribute
    {
        protected ThreadingBaseAspect() => Order = 200;

        [CanBeNull]
        public string HolderName { get; set; }

        protected bool IsInitialized { get; private set; }

        protected override void Initialize(object target, ObjectContext context, string contextName)
        {
            base.Initialize(target, context, contextName);

            IsInitialized = true;
        }
    }
}