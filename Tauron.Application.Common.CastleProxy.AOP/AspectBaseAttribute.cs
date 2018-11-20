using System;
using System.Reflection;
using Castle.DynamicProxy;
using JetBrains.Annotations;
using Tauron.Application.Common.CastleProxy.Impl.LifeTime;
using Tauron.Application.Ioc;
using Tauron.Application.Ioc.LifeTime;

namespace Tauron.Application.Common.CastleProxy.Aop
{
    public abstract class AspectBaseAttribute : MemberInterceptionAttribute, ISpecificInterceptor
    {
        void IInterceptor.Intercept(IInvocation invocation)
        {
            Intercept(invocation, ContextManager.FindContext(invocation.InvocationTarget, ContextName));
        }

        public override IInterceptor Create(MemberInfo info)
        {
            MemberType = info.MemberType;
            Name = info is Type ? AopConstants.InternalUniversalInterceptorName : info.Name;
            return this;
        }

        public string ContextName { get; private set; }

        public MemberTypes MemberType { get; private set; }

        public string Name { get; private set; } = string.Empty;

        public int Order { get; set; }

        protected override void Initialize([NotNull] object target, [NotNull] ObjectContext context, [NotNull] string contextName) => ContextName = contextName;

        protected virtual void Intercept([NotNull] IInvocation invocation, [NotNull] ObjectContext context)
        {
            Argument.NotNull(invocation, nameof(invocation));
            Argument.NotNull(context, nameof(context));

            invocation.Proceed();
        }
    }
}