// The file SemaphoreSync.cs is part of Tauron.Application.Common.
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
// <copyright file="SemaphoreSync.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The semaphore holder.
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
    /// <summary>The semaphore holder.</summary>
    public class SemaphoreHolder : BaseHolder<SemaphoreSlim>
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SemaphoreHolder" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="SemaphoreHolder" /> Klasse.
        ///     Initializes a new instance of the <see cref="SemaphoreHolder" /> class.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        public SemaphoreHolder([CanBeNull] SemaphoreSlim value)
            : base(value ?? new SemaphoreSlim(1))
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SemaphoreHolder" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="SemaphoreHolder" /> Klasse.
        ///     Initializes a new instance of the <see cref="SemaphoreHolder" /> class.
        /// </summary>
        public SemaphoreHolder()
            : base(new SemaphoreSlim(1))
        {
        }

        #endregion
    }

    /// <summary>The semaphore source attribute.</summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    [PublicAPI]
    public sealed class SemaphoreSourceAttribute : ContextPropertyAttributeBase
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
            context.Register<SemaphoreHolder, SemaphoreHolder>(
                new SemaphoreHolder(
                    info.GetInvokeMember<SemaphoreSlim>(target))
                {
                    Name
                        =
                        HolderName
                });
        }

        #endregion
    }

    /// <summary>The semaphore attribute.</summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Event, AllowMultiple = true,
        Inherited = true)]
    public sealed class SemaphoreAttribute : ThreadingBaseAspect
    {
        #region Fields

        /// <summary>The _holder.</summary>
        private SemaphoreHolder _holder;

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
            _holder = BaseHolder.GetOrAdd<SemaphoreHolder, SemaphoreHolder>(
                context,
                () => new SemaphoreHolder(),
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
            _holder.Value.Wait();
            try
            {
                invocation.Proceed();
            }
            finally
            {
                _holder.Value.Release();
            }
        }

        #endregion
    }
}