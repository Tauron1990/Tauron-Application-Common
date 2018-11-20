using System;
using System.Reflection;
using System.Threading;
using Castle.DynamicProxy;
using JetBrains.Annotations;
using Tauron.Application.Ioc.LifeTime;

namespace Tauron.Application.Common.CastleProxy.Aop.Threading
{
    public sealed class CountdownEventHolder : BaseHolder<CountdownEvent>
    {
        public CountdownEventHolder([CanBeNull] CountdownEvent value)
            : base(value ?? new CountdownEvent(1))
        {
        }

        public CountdownEventHolder()
            : base(new CountdownEvent(1))
        {
        }

    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public sealed class CountdownEventSourceAttribute : ContextPropertyAttributeBase
    {
        protected override void Register(ObjectContext context, MemberInfo info, object target)
        {
            context.Register<CountdownEventHolder, CountdownEventHolder>(
                new CountdownEventHolder(
                    info.GetInvokeMember<CountdownEvent>(target))
                {
                    Name = HolderName
                });
        }

    }

    public enum CountdownEventAction
    {
        Add,
        Signal,
        Wait
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event, AllowMultiple = true)]
    public sealed class CountdownEventAttribute : ThreadingBaseAspect
    {
        private CountdownEventHolder _holder;

        public CountdownEventAttribute() => Count = 1;

        public int Count { get; set; }

        public CountdownEventAction EventAction { get; set; }

        protected override void Initialize(object target, ObjectContext context, string contextName)
        {
            _holder = BaseHolder.GetOrAdd<CountdownEventHolder, CountdownEventHolder>(
                context,
                () => new CountdownEventHolder(),
                HolderName ?? String.Empty);

            base.Initialize(target, context, contextName);
        }

        protected override void Intercept(IInvocation invocation, ObjectContext context)
        {
            switch (EventAction)
            {
                case CountdownEventAction.Add:
                    _holder.Value.AddCount(Count);
                    break;
                case CountdownEventAction.Signal:
                    _holder.Value.Signal(Count);
                    break;
                case CountdownEventAction.Wait:
                    _holder.Value.Wait();
                    break;
            }

            invocation.Proceed();
        }
    }
}