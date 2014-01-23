// The file EventAspectAttribute.cs is part of Tauron.Application.Common.
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
// <copyright file="EventAspectAttribute.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The event aspect attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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
    /// <summary>The event aspect attribute.</summary>
    [AttributeUsage(AttributeTargets.Event, AllowMultiple = false)]
    [PublicAPI]
    public abstract class EventAspectAttribute : AspectBaseAttribute
    {
        #region Fields

        /// <summary>The _event info.</summary>
        private EventInfo mEventInfo;

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
            bool getter = name.StartsWith(AopConstants.EventAdder, StringComparison.Ordinal);
            if (mEventInfo == null)
            {
                name = name.Remove(0, getter ? AopConstants.EventAdder.Length : AopConstants.EventRemover.Length);

                mEventInfo = invocation.Method.DeclaringType.GetEvent(name, AopConstants.DefaultBindingFlags);
                Contract.Assert(mEventInfo != null);
            }

            if (getter) OnGet(invocation, context, mEventInfo);
            else OnSet(invocation, context, mEventInfo);
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
        /// <param name="eventInfo">
        ///     The event info.
        /// </param>
        protected virtual void OnGet(IInvocation invocation, ObjectContext context, EventInfo eventInfo)
        {
            Contract.Requires<ArgumentNullException>(invocation != null, "invocation");
            Contract.Requires<ArgumentNullException>(context != null, "context");
            Contract.Requires<ArgumentNullException>(eventInfo != null, "eventInfo");

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
        /// <param name="eventInfo">
        ///     The event info.
        /// </param>
        protected virtual void OnSet(IInvocation invocation, ObjectContext context, EventInfo eventInfo)
        {
            Contract.Requires<ArgumentNullException>(invocation != null, "invocation");
            Contract.Requires<ArgumentNullException>(context != null, "context");
            Contract.Requires<ArgumentNullException>(eventInfo != null, "eventInfo");

            invocation.Proceed();
        }

        #endregion
    }
}