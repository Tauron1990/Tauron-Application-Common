#region

using System;
using System.Diagnostics.Contracts;
using System.Security;
using System.Threading;
using Castle.DynamicProxy;
using Tauron.Application.Ioc.LifeTime;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Aop.Model
{
    /// <summary>The secured operation attribute.</summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event, AllowMultiple = true,
        Inherited = true)]
    [PublicAPI]
    public sealed class SecuredOperationAttribute : AspectBaseAttribute
    {
        #region Fields

        private readonly string _roles;

        #endregion

        #region Constructors and Destructors

        public SecuredOperationAttribute([NotNull] string roles)
        {
            Contract.Requires<ArgumentNullException>(roles != null, "roles");

            _roles = roles;
        }

        #endregion

        #region Public Properties

        [NotNull]
        public string Roles
        {
            get
            {
                Contract.Ensures(Contract.Result<string>() != null);

                return _roles;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The intercept impl.
        /// </summary>
        /// <param name="invocation">
        ///     The invocation.
        /// </param>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <exception cref="SecurityException">
        /// </exception>
        protected override void Intercept([NotNull] IInvocation invocation, [NotNull] ObjectContext context)
        {
            var able = invocation.InvocationTarget as ISecurable;
            if (able != null)
            {
                Contract.Assert(Thread.CurrentPrincipal != null);

// ReSharper disable once PossibleNullReferenceException
                if (!able.IsUserInRole(Thread.CurrentPrincipal.Identity, _roles))
                {
                    throw new SecurityException(
                        string.Format(
                            "The user {0} does not have the required permissions.",
                            Thread.CurrentPrincipal.Identity.Name));
                }
            }

            invocation.Proceed();
        }

        #endregion
    }
}