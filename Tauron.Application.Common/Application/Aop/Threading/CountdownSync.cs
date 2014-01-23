// The file CountdownSync.cs is part of Tauron.Application.Common.
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
// <copyright file="CountdownSync.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The countdown event holder.
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
    /// <summary>The countdown event holder.</summary>
    public sealed class CountdownEventHolder : BaseHolder<CountdownEvent>
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="CountdownEventHolder" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="CountdownEventHolder" /> Klasse.
        ///     Initializes a new instance of the <see cref="CountdownEventHolder" /> class.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        public CountdownEventHolder([CanBeNull] CountdownEvent value)
            : base(value ?? new CountdownEvent(1))
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CountdownEventHolder" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="CountdownEventHolder" /> Klasse.
        ///     Initializes a new instance of the <see cref="CountdownEventHolder" /> class.
        /// </summary>
        public CountdownEventHolder()
            : base(new CountdownEvent(1))
        {
        }

        #endregion
    }

    /// <summary>The countdown event source attribute.</summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public sealed class CountdownEventSourceAttribute : ContextPropertyAttributeBase
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
            context.Register<CountdownEventHolder, CountdownEventHolder>(
                new CountdownEventHolder(
                    info.GetInvokeMember<CountdownEvent>(target))
                {
                    Name
                        =
                        HolderName
                });
        }

        #endregion
    }

    /// <summary>The countdown event action.</summary>
    public enum CountdownEventAction
    {
        /// <summary>The add.</summary>
        Add,

        /// <summary>The signal.</summary>
        Signal,

        /// <summary>The wait.</summary>
        Wait
    }

    /// <summary>The countdown event attribute.</summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event, AllowMultiple = true,
        Inherited = true)]
    public sealed class CountdownEventAttribute : ThreadingBaseAspect
    {
        #region Fields

        /// <summary>The _holder.</summary>
        private CountdownEventHolder _holder;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="CountdownEventAttribute" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="CountdownEventAttribute" /> Klasse.
        ///     Initializes a new instance of the <see cref="CountdownEventAttribute" /> class.
        /// </summary>
        public CountdownEventAttribute()
        {
            Count = 1;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the count.</summary>
        /// <value>The count.</value>
        public int Count { get; set; }

        /// <summary>Gets or sets the event action.</summary>
        /// <value>The event action.</value>
        public CountdownEventAction EventAction { get; set; }

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
            _holder = BaseHolder.GetOrAdd<CountdownEventHolder, CountdownEventHolder>(
                context,
                () => new CountdownEventHolder(),
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
        /// <exception cref="ArgumentOutOfRangeException">
        /// </exception>
        protected override void Intercept(IInvocation invocation, ObjectContext context)
        {
            switch (EventAction)
            {
                case CountdownEventAction.Add:
                    _holder.Value.AddCount(Count);
                    break;
                case CountdownEventAction.Signal:
                    _holder.Value.Signal(Count);
                    break;
                case CountdownEventAction.Wait:
                    _holder.Value.Wait();
                    break;
            }

            invocation.Proceed();
        }

        #endregion
    }
}