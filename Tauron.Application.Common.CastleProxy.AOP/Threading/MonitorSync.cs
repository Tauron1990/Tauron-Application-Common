using System;
using System.Reflection;
using System.Threading;
using Castle.DynamicProxy;
using JetBrains.Annotations;
using Tauron.Application.Ioc.LifeTime;

namespace Tauron.Application.Common.CastleProxy.Aop.Threading
{
    [PublicAPI]
    public sealed class MonitorHolder : BaseHolder<object>
    {
        public MonitorHolder([CanBeNull] object value)
            : base(value ?? new object())
        {
        }
        public MonitorHolder()
            : base(new object())
        {
        }

    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Event)]
    [PublicAPI]
    public sealed class MonitorLockAttribute : ThreadingBaseAspect
    {
        private MonitorHolder _holder;

        protected override void Initialize(object target, ObjectContext context, string contextName)
        {
            _holder = BaseHolder.GetOrAdd<MonitorHolder, MonitorHolder>(
                context,
                () => new MonitorHolder(),
                HolderName ?? String.Empty);

            base.Initialize(target, context, contextName);
        }

        protected override void Intercept(IInvocation invocation, ObjectContext context)
        {
            var lockTaken = false;

            try
            {
                if (!Monitor.IsEntered(_holder.Value))
                    Monitor.Enter(_holder.Value, ref lockTaken);

                invocation.Proceed();
            }
            finally
            {
                if (lockTaken)
                    Monitor.Exit(_holder.Value);
            }
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    [PublicAPI]
    public sealed class MonitorSourceAttribute : ContextPropertyAttributeBase
    {
        protected override void Register(ObjectContext context, MemberInfo info, object target)
        {
            context.Register<MonitorHolder, MonitorHolder>(
                new MonitorHolder(info.GetInvokeMember<object>(target))
                {
                    Name = HolderName
                });
        }
    }
}