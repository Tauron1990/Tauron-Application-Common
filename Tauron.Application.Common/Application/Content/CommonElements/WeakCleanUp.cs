#region

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

        public WeakDelegate([NotNull] Delegate @delegate)
        {
            Contract.Requires<ArgumentNullException>(@delegate != null, "delegate");

            _method = @delegate.Method;

            if (!_method.IsStatic) _reference = new WeakReference(@delegate.Target);
        }

        public WeakDelegate([NotNull] MethodBase methodInfo, [NotNull] object target)
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
        public bool Equals([CanBeNull] WeakDelegate other)
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
        public override bool Equals([CanBeNull]object obj)
        {
            if (ReferenceEquals(null, obj)) return false;

            if (ReferenceEquals(this, obj)) return true;

            return obj is WeakDelegate && Equals((WeakDelegate) obj);
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
        [CanBeNull]
        public object Invoke([CanBeNull] params object[] parms)
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
        public static void RegisterAction([NotNull] Action action)
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
// ReSharper disable once ObjectCreationAsStatement
                new GCNotifier(resetEvent);

                while (true)
                {
                    resetEvent.WaitOne();

                    var dead = new List<WeakDelegate>();
                    foreach (WeakDelegate weakDelegate in Actions.ToArray())
                    {
                        Contract.Assume(weakDelegate != null);

                        if (weakDelegate != null && weakDelegate.IsAlive)
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

            private readonly AutoResetEvent _resetEvent;

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
                _resetEvent = resetEvent;
            }

            /// <summary>
            ///     Finalizes an instance of the <see cref="GCNotifier" /> class.
            ///     Finalisiert eine Instanz der <see cref="GCNotifier" /> Klasse.
            /// </summary>
            ~GCNotifier()
            {
                if (Environment.HasShutdownStarted) return;

                if (AppDomain.CurrentDomain.IsFinalizingForUnload()) return;

                _resetEvent.Set();
// ReSharper disable once ObjectCreationAsStatement
                new GCNotifier(_resetEvent);
            }

            #endregion
        }
    }
}