// The file ObjectContextPropertyattribute.cs is part of Tauron.Application.Common.
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
// <copyright file="ObjectContextPropertyattribute.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The object context property attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.Contracts;
using System.Reflection;
using Tauron.Application.Ioc.LifeTime;

#endregion

namespace Tauron.Application.Ioc
{
    /// <summary>The object context property attribute.</summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    [ContractClass(typeof (ObjectContextPropertyAttributeContracts))]
    public abstract class ObjectContextPropertyAttribute : Attribute
    {
        #region Methods

        /// <summary>
        ///     The register.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <param name="info">
        ///     The info.
        /// </param>
        /// <param name="target">
        ///     The target.
        /// </param>
        protected internal abstract void Register(ObjectContext context, MemberInfo info, object target);

        #endregion
    }

    [ContractClassFor(typeof (ObjectContextPropertyAttribute))]
    internal abstract class ObjectContextPropertyAttributeContracts : ObjectContextPropertyAttribute
    {
        #region Methods

        /// <summary>
        ///     The register.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <param name="info">
        ///     The info.
        /// </param>
        /// <param name="target">
        ///     The target.
        /// </param>
        protected internal override void Register(ObjectContext context, MemberInfo info, object target)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
            Contract.Requires<ArgumentNullException>(info != null, "info");
            Contract.Requires<ArgumentNullException>(target != null, "target");
        }

        #endregion
    }
}