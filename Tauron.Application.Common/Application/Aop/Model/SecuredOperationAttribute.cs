// The file SecuredOperationAttribute.cs is part of Tauron.Application.Common.
// 
// CoreEngine is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// CoreEngine is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//  
// You should have received a copy of the GNU General Public License
//  along with Tauron.Application.Common If not, see <http://www.gnu.org/licenses/>.

#region

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SecuredOperationAttribute.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The secured operation attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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

        private readonly string roles;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SecuredOperationAttribute" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="SecuredOperationAttribute" /> Klasse.
        /// </summary>
        /// <param name="roles">
        ///     The roles.
        /// </param>
        public SecuredOperationAttribute(string roles)
        {
            Contract.Requires<ArgumentNullException>(roles != null, "roles");

            this.roles = roles;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the roles.</summary>
        /// <value>The roles.</value>
        public string Roles
        {
            get
            {
                Contract.Ensures(Contract.Result<string>() != null);

                return roles;
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
        protected override void Intercept(IInvocation invocation, ObjectContext context)
        {
            var able = invocation.InvocationTarget as ISecurable;
            if (able != null)
            {
                Contract.Assert(Thread.CurrentPrincipal != null);

                if (!able.IsUserInRole(Thread.CurrentPrincipal.Identity, roles))
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