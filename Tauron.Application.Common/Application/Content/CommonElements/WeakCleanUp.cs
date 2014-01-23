// The file WeakCleanUp.cs is part of Tauron.Application.Common.
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
// <copyright file="WeakCleanUp.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The weak delegate.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The weak delegate.</summary>
    [PublicAPI]
    public sealed class WeakDelegate : IWeakReference, IEquatable<WeakDelegate>
    {
        #region Fields

        /// <summary>The _method.</summary>
        private readonly MethodBase _method;

        /// <summary>The _reference.</summary>
        private readonly WeakReference _reference;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="WeakDelegate" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="WeakDelegate" /> Klasse.
        ///     Initializes a new instance of the <see cref="WeakDelegate" /> class.
        /// </summary>
        /// <param name="delegate">
        ///     The delegate.
        /// </param>
        public WeakDelegate(Delegate @delegate)
        {
            Contract.Requires<ArgumentNullException>(@delegate != null, "delegate");

            _method = @delegate.Method;

            if (!_method.IsStatic) _reference = new WeakReference(@delegate.Target);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="WeakDelegate" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="WeakDelegate" /> Klasse.
        ///     Initializes a new instance of the <see cref="WeakDelegate" /> class.
        /// </summary>
        /// <param name="methodInfo">
        ///     The method info.
        /// </param>
        /// <param name="target">
        ///     The target.
        /// </param>
        public WeakDelegate(MethodBase methodInfo, object target)
        {
            Contract.Requires<ArgumentNullException>(methodInfo != null, "methodInfo");
            Contract.Requires<ArgumentNullException>(target != null, "target");

            _method = methodInfo;
            _reference = new WeakReference(target);
        }

        #endregion

        #region Public Properties

        /// <summary>Gets a value indicating whether is alive.</summary>
        /// <value>The is alive.</value>
        public bool IsAlive
        {
            get { return _reference == null || _reference.IsAlive; }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The equals.
        /// </summary>
        /// <param name="other">
        ///     The other.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool Equals(WeakDelegate other)
        {
            if (ReferenceEquals(null, other)) return false;

            if (ReferenceEquals(this, other)) return true;

            return other._reference.Target == _reference.Target && other._method == _method;
        }

        /// <summary>The ==.</summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static bool operator ==(WeakDelegate left, WeakDelegate right)
        {
            bool leftnull = ReferenceEquals(left, null);
            bool rightNull = ReferenceEquals(right, null);

            return !leftnull ? left.Equals(right) : rightNull;
        }

        /// <summary>The !=.</summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static bool operator !=(WeakDelegate left, WeakDelegate right)
        {
            bool leftnull = ReferenceEquals(left, null);
            bool rightNull = ReferenceEquals(right, null);

            if (!leftnull) return !left.Equals(right);
            return !rightNull;
        }

        /// <summary>
        ///     The equals.
        /// </summary>
        /// <param name="obj">
        ///     The obj.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (obj.GetType() != typeof (WeakDelegate)) return false;

            return Equals((WeakDelegate) obj);
        }

        /// <summary>The get hash code.</summary>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                object target;
                return (((target = _reference.Target) != null ? target.GetHashCode() : 0)*397)
                       ^ _method.GetHashCode();
            }
        }

        /// <summary>
        ///     The invoke.
        /// </summary>
        /// <param name="parms">
        ///     The parms.
        /// </param>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        public object Invoke(params object[] parms)
        {
            if (_method.IsStatic) return _method.Invoke(null, parms);

            object target = _reference.Target;
            return target == null ? null : _method.Invoke(target, parms);
        }

        #endregion
    }

    /////// <summary>The gc notification.</summary>
    ////[DebuggerNonUserCode]
    ////public sealed class GCNotification
    ////{
    ////    #region Fields

    ////    /// <summary>The _invoke clean up.</summary>
    ////    private readonly Action _invokeCleanUp;

    ////    #endregion

    ////    #region Constructors and Destructors

    ////    /// <summary>Initialisiert eine neue Instanz der <see cref="GCNotification"/> Klasse.
    ////    ///     Initializes a new instance of the <see cref="GCNotification"/> class.</summary>
    ////    /// <param name="invokeCleanUp">The invoke clean up.</param>
    ////    public GCNotification(Action invokeCleanUp)
    ////    {
    ////        this._invokeCleanUp = invokeCleanUp;
    ////    }

    ////    /// <summary>Finalisiert eine Instanz der <see cref="GCNotification"/> Klasse.
    ////    ///     Finalizes an instance of the <see cref="GCNotification"/> class.</summary>
    ////    ~GCNotification()
    ////    {
    ////        Task.Factory.StartNew(this._invokeCleanUp);
    ////    }

    ////    #endregion
    ////}

    /// <summary>The weak clean up.</summary>
    [PublicAPI]
    public static class WeakCleanUp
    {
        #region Constants

        /// <summary>WeakCleanUpExceptionPolicy.</summary>
        public const string WeakCleanUpExceptionPolicy = "WeakCleanUpExceptionPolicy";

        #endregion

        #region Static Fields

        /// <summary>The actions.</summary>
        private static readonly List<WeakDelegate> Actions = Initialize();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The register action.
        /// </summary>
        /// <param name="action">
        ///     The action.
        /// </param>
        public static void RegisterAction(Action action)
        {
            Contract.Requires<ArgumentNullException>(action != null, "action");

            lock (Actions) Actions.Add(new WeakDelegate(action));
        }

        #endregion

        #region Methods

        /// <summary>The initialize.</summary>
        /// <returns>The List.</returns>
        private static List<WeakDelegate> Initialize()
        {
            Task.Factory.StartNew(InvokeCleanUp, TaskCreationOptions.LongRunning);
            return new List<WeakDelegate>();
        }

        /// <summary>The invoke clean up.</summary>
        private static void InvokeCleanUp()
        {
            var resetEvent = new AutoResetEvent(false);
            try
            {
                new GCNotifier(resetEvent);

                while (true)
                {
                    resetEvent.WaitOne();

                    var dead = new List<WeakDelegate>();
                    foreach (WeakDelegate weakDelegate in Actions.ToArray())
                    {
                        Contract.Assume(weakDelegate != null);

                        if (weakDelegate.IsAlive)
                        {
                            try
                            {
                                weakDelegate.Invoke();
                            }
                            catch (Exception ex)
                            {
                                try
                                {
                                    ExceptionPolicy.HandleException(ex, WeakCleanUpExceptionPolicy);
                                }
                                catch (ExceptionHandlingException)
                                {
                                }
                            }
                        }
                        else dead.Add(weakDelegate);
                    }

                    dead.ForEach(del => Actions.Remove(del));
                }
            }
            finally
            {
                resetEvent.Dispose();
            }
        }

        #endregion

        private class GCNotifier
        {
            #region Fields

            private readonly AutoResetEvent resetEvent;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="GCNotifier" /> class.
            ///     Initialisiert eine neue Instanz der <see cref="GCNotifier" /> Klasse.
            /// </summary>
            /// <param name="resetEvent">
            ///     The reset event.
            /// </param>
            public GCNotifier(AutoResetEvent resetEvent)
            {
                this.resetEvent = resetEvent;
            }

            /// <summary>
            ///     Finalizes an instance of the <see cref="GCNotifier" /> class.
            ///     Finalisiert eine Instanz der <see cref="GCNotifier" /> Klasse.
            /// </summary>
            ~GCNotifier()
            {
                if (Environment.HasShutdownStarted) return;

                if (AppDomain.CurrentDomain.IsFinalizingForUnload()) return;

                resetEvent.Set();
                new GCNotifier(resetEvent);
            }

            #endregion
        }
    }
}