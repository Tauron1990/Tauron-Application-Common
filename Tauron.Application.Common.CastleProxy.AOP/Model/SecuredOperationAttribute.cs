using System;
using System.Security;
using System.Threading;
using Castle.DynamicProxy;
using JetBrains.Annotations;
using Tauron.Application.Ioc.LifeTime;

namespace Tauron.Application.Common.CastleProxy.Aop.Model
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event, AllowMultiple = true)]
    [PublicAPI]
    public sealed class SecuredOperationAttribute : AspectBaseAttribute
    {
        public SecuredOperationAttribute([NotNull] string roles) => Roles = Argument.NotNull(roles, nameof(roles));

        [NotNull]
        public string Roles { get; }
        
        protected override void Intercept(IInvocation invocation, ObjectContext context)
        {
            var able = invocation.InvocationTarget as ISecurable;

            if (!able?.IsUserInRole(Thread.CurrentPrincipal.Identity, Roles) == true)
            {
                throw new SecurityException(
                    $"The user {Thread.CurrentPrincipal.Identity.Name} does not have the required permissions.");
            }

            invocation.Proceed();
        }
    }
}