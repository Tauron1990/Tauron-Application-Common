// The file MethodAspectAttribute.cs is part of Tauron.Application.Common.
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
// <copyright file="MethodAspectAttribute.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The method aspect attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.Contracts;
using Castle.DynamicProxy;
using Tauron.Application.Ioc.LifeTime;

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
        protected virtual void EnterMethod(IInvocation invocation, ObjectContext context)
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
        protected virtual void ExecuteMethod(IInvocation invocation, ObjectContext context)
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
        protected virtual void ExitMethod(IInvocation invocation, ObjectContext context)
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
        protected virtual void FinallyMethod(IInvocation invocation, ObjectContext context)
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
                MethodException(invocation, e, context);
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
        protected virtual void MethodException(IInvocation invocation, Exception exception, ObjectContext context)
        {
            Contract.Requires<ArgumentNullException>(invocation != null, "invocation");
            Contract.Requires<ArgumentNullException>(context != null, "context");
            Contract.Requires<ArgumentNullException>(exception != null, "exception");
        }

        #endregion
    }
}