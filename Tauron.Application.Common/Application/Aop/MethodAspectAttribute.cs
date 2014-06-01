#region

using System;
using System.Diagnostics.Contracts;
using Castle.DynamicProxy;
using Tauron.Application.Ioc.LifeTime;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Aop
{
    /// <summary>The method aspect attribute.</summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public abstract class MethodAspectAttribute : AspectBaseAttribute
    {
        #region Methods

        /// <summary>
        ///     The enter method.
        /// </summary>
        /// <param name="invocation">
        ///     The invocation.
        /// </param>
        /// <param name="context">
        ///     The context.
        /// </param>
        protected virtual void EnterMethod([NotNull] IInvocation invocation, [NotNull] ObjectContext context)
        {
            Contract.Requires<ArgumentNullException>(invocation != null, "invocation");
            Contract.Requires<ArgumentNullException>(context != null, "context");
        }

        /// <summary>
        ///     The execute method.
        /// </summary>
        /// <param name="invocation">
        ///     The invocation.
        /// </param>
        /// <param name="context">
        ///     The context.
        /// </param>
        protected virtual void ExecuteMethod([NotNull] IInvocation invocation, [NotNull] ObjectContext context)
        {
            Contract.Requires<ArgumentNullException>(invocation != null, "invocation");
            Contract.Requires<ArgumentNullException>(context != null, "context");

            invocation.Proceed();
        }

        /// <summary>
        ///     The exit method.
        /// </summary>
        /// <param name="invocation">
        ///     The invocation.
        /// </param>
        /// <param name="context">
        ///     The context.
        /// </param>
        protected virtual void ExitMethod([NotNull] IInvocation invocation, [NotNull] ObjectContext context)
        {
            Contract.Requires<ArgumentNullException>(invocation != null, "invocation");
            Contract.Requires<ArgumentNullException>(context != null, "context");
        }

        /// <summary>
        ///     The finally method.
        /// </summary>
        /// <param name="invocation">
        ///     The invocation.
        /// </param>
        /// <param name="context">
        ///     The context.
        /// </param>
        protected virtual void FinallyMethod([NotNull] IInvocation invocation, [NotNull] ObjectContext context)
        {
            Contract.Requires<ArgumentNullException>(invocation != null, "invocation");
            Contract.Requires<ArgumentNullException>(context != null, "context");
        }

        /// <summary>
        ///     The intercept impl.
        /// </summary>
        /// <param name="invocation">
        ///     The invocation.
        /// </param>
        /// <param name="context">
        ///     The context.
        /// </param>
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
                if(MethodException(invocation, e, context))
                    throw;
            }
            finally
            {
                FinallyMethod(invocation, context);
            }
        }

        /// <summary>
        ///     The method exception.
        /// </summary>
        /// <param name="invocation">
        ///     The invocation.
        /// </param>
        /// <param name="exception">
        ///     The e.
        /// </param>
        /// <param name="context">
        ///     The context.
        /// </param>
        protected virtual bool MethodException([NotNull] IInvocation invocation, [NotNull] Exception exception, [NotNull] ObjectContext context)
        {
            Contract.Requires<ArgumentNullException>(invocation != null, "invocation");
            Contract.Requires<ArgumentNullException>(context != null, "context");
            Contract.Requires<ArgumentNullException>(exception != null, "exception");

            return true;
        }

        #endregion
    }
}