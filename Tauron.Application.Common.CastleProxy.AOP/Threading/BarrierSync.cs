using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading;
using Castle.DynamicProxy;
using JetBrains.Annotations;
using Tauron.Application.Ioc.LifeTime;

namespace Tauron.Application.Common.CastleProxy.Aop.Threading
{
    public class BarrierHolder : BaseHolder<Barrier>
    {
        [SuppressMessage("Microsoft.Reliability", "CA2000:Objekte verwerfen, bevor Bereich verloren geht",
            Justification = "Used In Upper Scope")]
        public BarrierHolder([CanBeNull] Barrier value)
            : base(value ?? new Barrier(1))
        {
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Objekte verwerfen, bevor Bereich verloren geht",
            Justification = "Used In Upper Scope")]
        public BarrierHolder()
            : base(new Barrier(1))
        {
        }

    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    [PublicAPI]
    public sealed class BarrierSourceAttribute : ContextPropertyAttributeBase
    {
        protected override void Register(ObjectContext context, MemberInfo info, object target)
        {
            context.Register<BarrierHolder, BarrierHolder>(
                new BarrierHolder(info.GetInvokeMember<Barrier>(target))
                {
                    Name = HolderName
                });
        }

    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Event)]
    [PublicAPI]
    public sealed class BarrierAttribute : ThreadingBaseAspect
    {
        private BarrierHolder _holder;

        public MethodInvocationPosition Position { get; set; }

        protected override void Initialize(object target, ObjectContext context, string contextName)
        {
            _holder = BaseHolder.GetOrAdd<BarrierHolder, BarrierHolder>(
                context,
                () => new BarrierHolder(),
                HolderName ?? String.Empty);

            base.Initialize(target, context, contextName);
        }

        protected override void Intercept(IInvocation invocation, ObjectContext context)
        {
            if (Position == MethodInvocationPosition.Before) _holder.Value.SignalAndWait();

            invocation.Proceed();

            if (Position == MethodInvocationPosition.After) _holder.Value.SignalAndWait();
        }
    }
}