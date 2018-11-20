using System;
using System.Reflection;
using System.Threading;
using Castle.DynamicProxy;
using JetBrains.Annotations;
using Tauron.Application.Ioc.LifeTime;

namespace Tauron.Application.Common.CastleProxy.Aop.Threading
{
    [PublicAPI]
    public class ManualResetEventHolder : BaseHolder<ManualResetEventSlim>
    {
        public ManualResetEventHolder([CanBeNull] ManualResetEventSlim value)
            : base(value ?? new ManualResetEventSlim())
        {
        }

        public ManualResetEventHolder()
            : base(new ManualResetEventSlim())
        {
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public sealed class ManualResetEventSourceAttribute : ContextPropertyAttributeBase
    {
        protected override void Register(ObjectContext context, MemberInfo info, object target)
        {
            context.Register<ManualResetEventHolder, ManualResetEventHolder>(
                new ManualResetEventHolder(
                    info
                        .GetInvokeMember
                            <ManualResetEventSlim>(target))
                {
                    Name = HolderName
                });
        }
    }

    [PublicAPI]
    public enum MethodInvocationPosition
    {
        Before,
        After
    }

    [PublicAPI]
    public enum ManualResetEventBehavior
    {
        Set,
        Wait
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Event | AttributeTargets.Property)]
    public sealed class ManualResetEventAttribute : ThreadingBaseAspect
    {
        private ManualResetEventHolder _holder;

        public ManualResetEventAttribute()
        {
            Position = MethodInvocationPosition.Before;
            EventBehavior = ManualResetEventBehavior.Wait;
        }

        public ManualResetEventBehavior EventBehavior { get; set; }

        public MethodInvocationPosition Position { get; set; }

        protected override void Initialize(object target, ObjectContext context, string contextName)
        {
            _holder = BaseHolder.GetOrAdd<ManualResetEventHolder, ManualResetEventHolder>(
                context,
                () => new ManualResetEventHolder(), HolderName ?? String.Empty);

            base.Initialize(target, context, contextName);
        }

        protected override void Intercept(IInvocation invocation, ObjectContext context)
        {
            if (Position == MethodInvocationPosition.Before)
            {
                if (EventBehavior == ManualResetEventBehavior.Wait)
                {
                    _holder.Value.Wait();
                }
                else
                {
                    _holder.Value.Set();
                    _holder.Value.Reset();
                }
            }

            invocation.Proceed();

            if (EventBehavior == ManualResetEventBehavior.Wait)
            {
                _holder.Value.Wait();
            }
            else
            {
                _holder.Value.Set();
                _holder.Value.Reset();
            }
        }
    }
}