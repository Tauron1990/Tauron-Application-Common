﻿// The file MemberInterceptionAttribute.cs is part of Tauron.Application.Common.
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
// <copyright file="MemberInterceptionAttribute.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The member interception attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.Contracts;
using System.Reflection;
using Castle.DynamicProxy;
using Tauron.Application.Ioc.LifeTime;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Ioc
{
    /// <summary>The member interception attribute.</summary>
    [AttributeUsage(
        AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event | AttributeTargets.Class,
        AllowMultiple = true)]
    [PublicAPI]
    [ContractClass(typeof (MemberInterceptionAttributeContracts))]
    public abstract class MemberInterceptionAttribute : Attribute
    {
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
        public abstract IInterceptor Create(MemberInfo info);

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
        protected internal abstract void Initialize(object target, ObjectContext context, string contextName);

        #endregion
    }

    [ContractClassFor(typeof (MemberInterceptionAttribute))]
    internal abstract class MemberInterceptionAttributeContracts : MemberInterceptionAttribute
    {
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
            Contract.Requires<ArgumentNullException>(info != null, "info");
            Contract.Ensures(Contract.Result<IInterceptor>() != null);
            return null;
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
            Contract.Requires<ArgumentNullException>(target != null, "target");
            Contract.Requires<ArgumentNullException>(context != null, "context");
            Contract.Requires<ArgumentNullException>(contextName != null, "contextName");
        }

        #endregion
    }
}