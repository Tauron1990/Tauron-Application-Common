// The file ManualResetEventSync.cs is part of Tauron.Application.Common.
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
// <copyright file="ManualResetEventSync.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The manual reset event holder.
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
    /// <summary>The manual reset event holder.</summary>
    [PublicAPI]
    public class ManualResetEventHolder : BaseHolder<ManualResetEventSlim>
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ManualResetEventHolder" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="ManualResetEventHolder" /> Klasse.
        ///     Initializes a new instance of the <see cref="ManualResetEventHolder" /> class.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        public ManualResetEventHolder([CanBeNull] ManualResetEventSlim value)
            : base(value ?? new ManualResetEventSlim())
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ManualResetEventHolder" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="ManualResetEventHolder" /> Klasse.
        ///     Initializes a new instance of the <see cref="ManualResetEventHolder" /> class.
        /// </summary>
        public ManualResetEventHolder()
            : base(new ManualResetEventSlim())
        {
        }

        #endregion
    }

    /// <summary>The manual reset event source attribute.</summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public sealed class ManualResetEventSourceAttribute : ContextPropertyAttributeBase
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
            context.Register<ManualResetEventHolder, ManualResetEventHolder>(
                new ManualResetEventHolder(
                    info
                        .GetInvokeMember
                        <ManualResetEventSlim>(target))
                {
                    Name = HolderName
                });
        }

        #endregion
    }

    /// <summary>The method invocation position.</summary>
    [PublicAPI]
    public enum MethodInvocationPosition
    {
        /// <summary>The before.</summary>
        Before,

        /// <summary>The after.</summary>
        After
    }

    /// <summary>The manual reset event behavior.</summary>
    [PublicAPI]
    public enum ManualResetEventBehavior
    {
        /// <summary>The set.</summary>
        Set,

        /// <summary>The wait.</summary>
        Wait
    }

    /// <summary>The manual reset event attribute.</summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Event | AttributeTargets.Property, AllowMultiple = false,
        Inherited = true)]
    public sealed class ManualResetEventAttribute : ThreadingBaseAspect
    {
        #region Fields

        /// <summary>The _holder.</summary>
        private ManualResetEventHolder _holder;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ManualResetEventAttribute" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="ManualResetEventAttribute" /> Klasse.
        ///     Initializes a new instance of the <see cref="ManualResetEventAttribute" /> class.
        /// </summary>
        public ManualResetEventAttribute()
        {
            Position = MethodInvocationPosition.Before;
            EventBehavior = ManualResetEventBehavior.Wait;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the event behavior.</summary>
        /// <value>The event behavior.</value>
        public ManualResetEventBehavior EventBehavior { get; set; }

        /// <summary>Gets or sets the position.</summary>
        /// <value>The position.</value>
        public MethodInvocationPosition Position { get; set; }

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
            _holder = BaseHolder.GetOrAdd<ManualResetEventHolder, ManualResetEventHolder>(
                context,
                () =>
                new ManualResetEventHolder(),
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
        protected override void Intercept(IInvocation invocation, ObjectContext context)
        {
            if (Position == MethodInvocationPosition.Before)
            {
                if (EventBehavior == ManualResetEventBehavior.Wait) _holder.Value.Wait();
                else
                {
                    _holder.Value.Set();
                    _holder.Value.Reset();
                }
            }

            invocation.Proceed();

            if (EventBehavior == ManualResetEventBehavior.Wait) _holder.Value.Wait();
            else
            {
                _holder.Value.Set();
                _holder.Value.Reset();
            }
        }

        #endregion
    }
}