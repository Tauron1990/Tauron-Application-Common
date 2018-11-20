using System;
using Castle.DynamicProxy;
using JetBrains.Annotations;
using Tauron.Application.Ioc.LifeTime;

namespace Tauron.Application.Common.CastleProxy.Aop
{
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class MethodAspectAttribute : AspectBaseAttribute
    {
        protected virtual void EnterMethod([NotNull] IInvocation invocation, [NotNull] ObjectContext context) { }

        protected virtual void ExecuteMethod([NotNull] IInvocation invocation, [NotNull] ObjectContext context)
        {
            invocation.Proceed();
        }
        
        protected virtual void ExitMethod([NotNull] IInvocation invocation, [NotNull] ObjectContext context) { }

        protected virtual void FinallyMethod([NotNull] IInvocation invocation, [NotNull] ObjectContext context) { }
        
        protected override void Intercept(IInvocation invocation, ObjectContext context)
        {
            try
            {
                EnterMethod(invocation, context);
                ExecuteMethod(invocation, context);
                ExitMethod(invocation, context);
            }
            catch (Exception e)
            {
                if (MethodException(invocation, e, context))
                    throw;
            }
            finally
            {
                FinallyMethod(invocation, context);
            }
        }
        
        protected virtual bool MethodException([NotNull] IInvocation invocation, [NotNull] Exception exception, [NotNull] ObjectContext context) => true;
    }
}