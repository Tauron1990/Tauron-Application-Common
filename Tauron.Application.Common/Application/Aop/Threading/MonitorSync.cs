// The file MonitorSync.cs is part of Tauron.Application.Common.
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
// <copyright file="MonitorSync.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The monitor holder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Reflection;
using System.Threading;
using Castle.DynamicProxy;
using Tauron.Application.Ioc.LifeTime;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Aop.Threading
{
    /// <summary>The monitor holder.</summary>
    [PublicAPI]
    public sealed class MonitorHolder : BaseHolder<object>
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MonitorHolder" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="MonitorHolder" /> Klasse.
        ///     Initializes a new instance of the <see cref="MonitorHolder" /> class.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        public MonitorHolder([CanBeNull] object value)
            : base(value ?? new object())
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MonitorHolder" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="MonitorHolder" /> Klasse.
        ///     Initializes a new instance of the <see cref="MonitorHolder" /> class.
        /// </summary>
        public MonitorHolder()
            : base(new object())
        {
        }

        #endregion
    }

    /// <summary>The monitor lock attribute.</summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Event, AllowMultiple = false,
        Inherited = true)]
    [PublicAPI]
    public sealed class MonitorLockAttribute : ThreadingBaseAspect
    {
        #region Fields

        /// <summary>The _holder.</summary>
        private MonitorHolder _holder;

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
            _holder = BaseHolder.GetOrAdd<MonitorHolder, MonitorHolder>(
                context,
                () => new MonitorHolder(),
                HolderName);

            base.Initialize(target, context, contextName);
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
        protected override void Intercept([NotNull] IInvocation invocation, [NotNull] ObjectContext context)
        {
            bool lockTaken = false;

            try
            {
                if (!Monitor.IsEntered(_holder.Value)) Monitor.Enter(_holder.Value, ref lockTaken);

                invocation.Proceed();
            }
            finally
            {
                if (lockTaken) Monitor.Exit(_holder.Value);
            }
        }

        #endregion
    }

    /// <summary>The monitor source attribute.</summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    [PublicAPI]
    public sealed class MonitorSourceAttribute : ContextPropertyAttributeBase
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
        protected internal override void Register([NotNull] ObjectContext context, [NotNull] MemberInfo info, [NotNull] object target)
        {
            context.Register<MonitorHolder, MonitorHolder>(
                new MonitorHolder(info.GetInvokeMember<object>(target))
                {
                    Name
                        =
                        HolderName
                });
        }

        #endregion
    }
}