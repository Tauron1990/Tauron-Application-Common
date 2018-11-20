using System;
using System.Reflection;
using System.Threading;
using Castle.DynamicProxy;
using JetBrains.Annotations;
using Tauron.Application.Ioc.LifeTime;

namespace Tauron.Application.Common.CastleProxy.Aop.Threading
{
    public class SemaphoreHolder : BaseHolder<SemaphoreSlim>
    {
        public SemaphoreHolder([CanBeNull] SemaphoreSlim value)
            : base(value ?? new SemaphoreSlim(1))
        {
        }

        public SemaphoreHolder()
            : base(new SemaphoreSlim(1))
        {
        }

    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    [PublicAPI]
    public sealed class SemaphoreSourceAttribute : ContextPropertyAttributeBase
    {
        protected override void Register(ObjectContext context, MemberInfo info, object target)
        {
            context.Register<SemaphoreHolder, SemaphoreHolder>(
                new SemaphoreHolder(
                    info.GetInvokeMember<SemaphoreSlim>(target))
                {
                    Name
                        =
                        HolderName
                });
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Event, AllowMultiple = true)]
    public sealed class SemaphoreAttribute : ThreadingBaseAspect
    {
        private SemaphoreHolder _holder;

        protected override void Initialize(object target, ObjectContext context, string contextName)
        {
            _holder = BaseHolder.GetOrAdd<SemaphoreHolder, SemaphoreHolder>(
                context,
                () => new SemaphoreHolder(),
                HolderName ?? string.Empty);

            base.Initialize(target, context, contextName);
        }

        protected override void Intercept(IInvocation invocation, ObjectContext context)
        {
            _holder.Value.Wait();
            try
            {
                invocation.Proceed();
            }
            finally
            {
                _holder.Value.Release();
            }
        }
    }
}