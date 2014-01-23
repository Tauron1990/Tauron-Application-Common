// The file PropertyAspectAttrebute.cs is part of Tauron.Application.Common.
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
// <copyright file="PropertyAspectAttrebute.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The property aspect attrebute.
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
    /// <summary>The property aspect attrebute.</summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public abstract class PropertyAspectAttrebute : AspectBaseAttribute
    {
        #region Fields

        /// <summary>The _property info.</summary>
        private PropertyInfo mpropertyInfo;

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
        protected override void Intercept(IInvocation invocation, ObjectContext context)
        {
            string name = invocation.Method.Name;
            bool getter = name.StartsWith(AopConstants.PropertyGetter, StringComparison.Ordinal);
            if (mpropertyInfo == null)
            {
                name = name.Remove(0, getter ? AopConstants.PropertyGetter.Length : AopConstants.PropertySetter.Length);

                mpropertyInfo = invocation.Method.DeclaringType.GetProperty(name, AopConstants.DefaultBindingFlags);
                Contract.Assert(mpropertyInfo != null);
            }

            if (getter) OnGet(invocation, context, mpropertyInfo);
            else OnSet(invocation, context, mpropertyInfo);
        }

        /// <summary>
        ///     The on get.
        /// </summary>
        /// <param name="invocation">
        ///     The invocation.
        /// </param>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <param name="propertyInfo">
        ///     The property.
        /// </param>
        protected virtual void OnGet(IInvocation invocation, ObjectContext context, PropertyInfo propertyInfo)
        {
            Contract.Requires<ArgumentNullException>(invocation != null, "invocation");
            Contract.Requires<ArgumentNullException>(context != null, "context");
            Contract.Requires<ArgumentNullException>(propertyInfo != null, "propertyInfo");

            invocation.Proceed();
        }

        /// <summary>
        ///     The on set.
        /// </summary>
        /// <param name="invocation">
        ///     The invocation.
        /// </param>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <param name="propertyInfo">
        ///     The property.
        /// </param>
        protected virtual void OnSet(IInvocation invocation, ObjectContext context, PropertyInfo propertyInfo)
        {
            Contract.Requires<ArgumentNullException>(invocation != null, "invocation");
            Contract.Requires<ArgumentNullException>(context != null, "context");
            Contract.Requires<ArgumentNullException>(propertyInfo != null, "propertyInfo");

            invocation.Proceed();
        }

        #endregion
    }
}