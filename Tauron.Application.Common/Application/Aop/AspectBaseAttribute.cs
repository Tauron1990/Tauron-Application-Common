#region

using System;
using System.Diagnostics.Contracts;
using System.Reflection;
using Castle.DynamicProxy;
using Tauron.Application.Ioc;
using Tauron.Application.Ioc.LifeTime;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Aop
{
    /// <summary>The aspect base attribute.</summary>
    public abstract class AspectBaseAttribute : MemberInterceptionAttribute, ISpecificInterceptor
    {
        #region Fields

        private string name = string.Empty;

        #endregion

        #region Public Properties

        /// <summary>Gets the context name.</summary>
        /// <value>The context name.</value>
        public string ContextName { get; private set; }

        /// <summary>Gets the member type.</summary>
        /// <value>The member type.</value>
        public MemberTypes MemberType { get; private set; }

        /// <summary>Gets the name.</summary>
        /// <value>The name.</value>
        public string Name
        {
            get
            {
                Contract.Ensures(Contract.Result<string>() != null);

                return name;
            }

            private set
            {
                Contract.Requires<ArgumentNullException>(value != null, "value");

                name = value;
            }
        }

        /// <summary>Gets or sets the order.</summary>
        /// <value>The order.</value>
        public int Order { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The create.
        /// </summary>
        /// <param name="info">
        ///     The info.
        /// </param>
        /// <returns>
        ///     The <see cref="IInterceptor" />.
        /// </returns>
        public override IInterceptor Create(MemberInfo info)
        {
            MemberType = info.MemberType;
            Name = info is Type ? AopConstants.InternalUniversalInterceptorName : info.Name;
            return this;
        }

        #endregion

        #region Explicit Interface Methods

        /// <summary>
        ///     The intercept.
        /// </summary>
        /// <param name="invocation">
        ///     The invocation.
        /// </param>
        void IInterceptor.Intercept(IInvocation invocation)
        {
            Intercept(invocation, ContextManager.FindContext(invocation.InvocationTarget, ContextName));
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The initialize.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <param name="contextName">
        ///     The context name.
        /// </param>
        protected internal override void Initialize([NotNull] object target, [NotNull] ObjectContext context, [NotNull] string contextName)
        {
            ContextName = contextName;
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
        protected virtual void Intercept([NotNull] IInvocation invocation, [NotNull] ObjectContext context)
        {
            Contract.Requires<ArgumentNullException>(invocation != null, "invocation");
            Contract.Requires<ArgumentNullException>(context != null, "context");

            invocation.Proceed();
        }

        #endregion
    }
}