// The file ReaderWriterLockSync.cs is part of Tauron.Application.Common.
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
// <copyright file="ReaderWriterLockSync.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The reader writer lock holder.
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
    /// <summary>The reader writer lock holder.</summary>
    public class ReaderWriterLockHolder : BaseHolder<ReaderWriterLockSlim>
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReaderWriterLockHolder" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="ReaderWriterLockHolder" /> Klasse.
        ///     Initializes a new instance of the <see cref="ReaderWriterLockHolder" /> class.
        /// </summary>
        /// <param name="lockSlim">
        ///     The lock slim.
        /// </param>
        public ReaderWriterLockHolder([CanBeNull] ReaderWriterLockSlim lockSlim)
            : base(lockSlim ?? new ReaderWriterLockSlim())
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReaderWriterLockHolder" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="ReaderWriterLockHolder" /> Klasse.
        ///     Initializes a new instance of the <see cref="ReaderWriterLockHolder" /> class.
        /// </summary>
        public ReaderWriterLockHolder()
            : base(new ReaderWriterLockSlim())
        {
        }

        #endregion
    }

    /// <summary>The reader writer lock source attribute.</summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    [PublicAPI]
    public sealed class ReaderWriterLockSourceAttribute : ContextPropertyAttributeBase
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
            context.Register<ReaderWriterLockHolder, ReaderWriterLockHolder>(
                new ReaderWriterLockHolder(
                    info
                        .GetInvokeMember
                        <ReaderWriterLockSlim>(target))
                {
                    Name = HolderName,
                });
        }

        #endregion
    }

    /// <summary>The reader writer lock behavior.</summary>
    [PublicAPI]
    public enum ReaderWriterLockBehavior
    {
        Invalid,
        /// <summary>The read.</summary>
        Read,

        /// <summary>The write.</summary>
        Write,
    }

    /// <summary>The reader writer lock attribute.</summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Event | AttributeTargets.Property, AllowMultiple = false,
        Inherited = true)]
    public sealed class ReaderWriterLockAttribute : ThreadingBaseAspect
    {
        #region Fields

        /// <summary>The _enter.</summary>
        private Func<bool> _enter;

        /// <summary>The _exit.</summary>
        private Action _exit;

        /// <summary>The _holder.</summary>
        private ReaderWriterLockHolder _holder;

        /// <summary>The _skip.</summary>
        private Func<bool> _skip;

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the lock behavior.</summary>
        /// <value>The lock behavior.</value>
        public ReaderWriterLockBehavior LockBehavior { get; set; }

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
        /// <exception cref="ArgumentOutOfRangeException">
        /// </exception>
        protected internal override void Initialize([NotNull] object target, [NotNull] ObjectContext context, [NotNull] string contextName)
        {
            _holder = BaseHolder.GetOrAdd<ReaderWriterLockHolder, ReaderWriterLockHolder>(
                context,
                () =>
                new ReaderWriterLockHolder(),
                HolderName);
            switch (LockBehavior)
            {
                case ReaderWriterLockBehavior.Read:
                    _enter = () => _holder.Value.TryEnterReadLock(-1);
                    _exit = _holder.Value.ExitReadLock;
                    _skip = () => _holder.Value.IsReadLockHeld;
                    break;
                case ReaderWriterLockBehavior.Write:
                    _enter = () => _holder.Value.TryEnterWriteLock(-1);
                    _exit = _holder.Value.ExitWriteLock;
                    _skip = () => _holder.Value.IsWriteLockHeld;
                    break;
                default:
                    CommonConstants.LogCommon(false, "AOP Module: Invalid Reader WriterLogBahavior: {0}", target);
                    break;
            }

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
            if(_skip == null) return;

            bool skipping = _skip();
            bool taken = false;

            try
            {
                if (!skipping) taken = _enter();

                invocation.Proceed();
            }
            finally
            {
                if (taken) _exit();
            }
        }

        #endregion
    }
}