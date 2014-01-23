// The file AspectBaseAttribute.cs is part of Tauron.Application.Common.
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
// <copyright file="AspectBaseAttribute.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The aspect base attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.Contracts;
using System.Reflection;
using Castle.DynamicProxy;
using Tauron.Application.Ioc;
using Tauron.Application.Ioc.LifeTime;

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
        protected internal override void Initialize(object target, ObjectContext context, string contextName)
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
        protected virtual void Intercept(IInvocation invocation, ObjectContext context)
        {
            Contract.Requires<ArgumentNullException>(invocation != null, "invocation");
            Contract.Requires<ArgumentNullException>(context != null, "context");

            invocation.Proceed();
        }

        #endregion
    }
}