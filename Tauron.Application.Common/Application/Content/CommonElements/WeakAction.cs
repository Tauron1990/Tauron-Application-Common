#region

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>Speichert informationen für eine Aktion ohne Refernce auf das Object.</summary>
    [PublicAPI]
    public sealed class WeakAction
    {
        private bool Equals([NotNull] WeakAction other)
        {
            return Equals(_method, other._method) && Equals(TargetObject.Target, other.TargetObject.Target);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_method != null ? _method.GetHashCode() : 0)*397) ^ (TargetObject.Target != null ? TargetObject.Target.GetHashCode() : 0);
            }
        }

        #region Fields

        /// <summary>The _delegate type.</summary>
        private readonly Type _delegateType;

        /// <summary>The _method.</summary>
        private readonly MethodInfo _method;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        /// <param name="target">
        ///     Das Ziel Object.
        /// </param>
        /// <param name="method">
        ///     Die Methode die Ausgeführt werden soll.
        /// </param>
        /// <param name="parameterType">
        ///     Der Parameter der methode.
        /// </param>
        [ContractVerification(false)]
        public WeakAction([CanBeNull] object target, [NotNull] MethodInfo method, [CanBeNull] Type parameterType)
        {
            Contract.Requires<ArgumentNullException>(method != null, "method");

            if (target != null) TargetObject = new WeakReference(target);

            _method = method;
            _delegateType = parameterType == null
                                ? typeof (Action)
                                : typeof (Action<>).MakeGenericType(parameterType);

            ParameterCount = parameterType == null ? 0 : 1;
        }

        /// <summary>
        ///     Initialisiert eine Instanz der Klasse mit einer methode mit Maximal 16 Parameter.
        /// </summary>
        /// <param name="target">
        ///     Das Object mit dem die Methode Ausgeführt werden soll.
        /// </param>
        /// <param name="method">
        ///     Die Methode die AUsgefürt werden soll.
        /// </param>
        public WeakAction([CanBeNull] object target, [NotNull] MethodInfo method)
        {
            Contract.Requires<ArgumentNullException>(method != null, "method");

            if (target != null) TargetObject = new WeakReference(target);

            Type[] parames =
                method.GetParameters().OrderBy(parm => parm.Position).Select(parm => parm.ParameterType).ToArray();
            Type returntype = method.ReturnType;
            _delegateType = returntype == typeof (void)
                                ? FactoryDelegateType("System.Action", parames.ToArray())
                                : FactoryDelegateType("System.Func", parames.Concat(new[] {returntype}).ToArray());

            ParameterCount = parames.Length;
        }

        #endregion

        #region Public Properties

        public int ParameterCount { get; private set; }

        /// <summary>Die ZielMethode.</summary>
        /// <value>The method info.</value>
        [NotNull]
        public MethodInfo MethodInfo
        {
            get
            {
                Contract.Ensures(Contract.Result<MethodInfo>() != null);

                return _method;
            }
        }

        /// <summary>Das Object mit dem die Methode Ausgeführt werden soll.</summary>
        /// <value>The target object.</value>
        [CanBeNull]
        public WeakReference TargetObject { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Führt die Methode aus.
        /// </summary>
        /// <param name="parms">
        ///     Die Argumente die Übergeben werden.
        /// </param>
        /// <returns>
        ///     Das Ergebnis der Methode.
        /// </returns>
        [CanBeNull]
        public object Invoke([NotNull] params object[] parms)
        {
            Delegate temp = CreateDelegate();
            return temp != null ? temp.DynamicInvoke(parms) : null;
        }

        #endregion

        #region Methods

        /// <summary>Creates callback delegate.</summary>
        /// <returns>Callback delegate.</returns>
        [CanBeNull]
        internal Delegate CreateDelegate()
        {
            if (TargetObject == null) return null;

            object target = TargetObject.Target;
            return target != null
                       ? Delegate.CreateDelegate(_delegateType, TargetObject.Target, _method)
                       : null;
        }

        /// <summary>
        ///     The factory delegate type.
        /// </summary>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <param name="types">
        ///     The types.
        /// </param>
        /// <returns>
        ///     The <see cref="Type" />.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        [NotNull]
        private static Type FactoryDelegateType([NotNull] string name, [NotNull] Type[] types)
        {
            Contract.Requires<ArgumentNullException>(name != null, "name");
            Contract.Requires<ArgumentNullException>(types != null, "types");

            Type type = Type.GetType(name + "`" + types.Length);
            if (type != null) return types.Length > 0 ? type.MakeGenericType(types) : Type.GetType(name);

            throw new InvalidOperationException();
        }

        #endregion

        public override bool Equals([CanBeNull] object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is WeakAction && Equals((WeakAction) obj);
        }
    }

    /// <summary>
    ///     Sammelt eine Gruppe von WeakAction Handlern und führ sie Gleichzeitig aus.
    /// </summary>
    /// <typeparam name="T">
    ///     Der Type der Des Parameters.
    /// </typeparam>
    [PublicAPI]
    public class WeakActionEvent<T>
    {
        #region Fields

        /// <summary>The _delegates.</summary>
        private readonly List<WeakAction> _delegates = new List<WeakAction>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="WeakActionEvent{T}" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="WeakActionEvent{T}" /> Klasse.
        ///     Initializes a new instance of the <see cref="WeakActionEvent{T}" /> class.
        /// </summary>
        public WeakActionEvent()
        {
            WeakCleanUp.RegisterAction(CleanUp);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// </summary>
        /// <param name="handler">
        /// </param>
        /// <returns>
        ///     The WeakActionEvent.
        /// </returns>
        [NotNull]
        public WeakActionEvent<T> Add([NotNull] Action<T> handler)
        {
            Contract.Requires<ArgumentNullException>(handler != null, "handler");

            ParameterInfo[] parameters = handler.Method.GetParameters();

            if (
                _delegates.Where(del => del.MethodInfo == handler.Method)
                          .Select(weakAction => weakAction.TargetObject != null ? weakAction.TargetObject.Target : null)
                          .Any(weakTarget => weakTarget == handler.Target)) return this;

            Contract.Assume(parameters.Length > 0);

            Type parameterType = parameters[0].ParameterType;

            lock (this)
            {
                _delegates.Add(new WeakAction(handler.Target, handler.Method, parameterType));
            }

            return this;
        }

        /// <summary>
        ///     Führt alle Handler aus und entfehrt dabei alle Toton Eiträge.
        /// </summary>
        /// <param name="arg">
        ///     Des Argument das Allen übergeben werden soll.
        /// </param>
        public void Invoke(T arg)
        {
            lock (this)
            {
                foreach (Delegate action in _delegates.Select(weakAction => weakAction.CreateDelegate())) action.DynamicInvoke(arg);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="handler">
        /// </param>
        /// <returns>
        ///     The WeakActionEvent.
        /// </returns>
        [NotNull]
        public WeakActionEvent<T> Remove([NotNull] Action<T> handler)
        {
            Contract.Requires<ArgumentNullException>(handler != null, "handler");

            lock (this)
            {
                foreach (WeakAction del in _delegates.Where(del => del.TargetObject != null && del.TargetObject.Target == handler.Target))
                {
                    _delegates.Remove(del);
                    return this;
                }
            }

            return this;
        }

        #endregion

        #region Methods

        /// <summary>The clean up.</summary>
        private void CleanUp()
        {
            Contract.Requires(_delegates != null);

            List<WeakAction> dead = _delegates.Where(item => !item.TargetObject.IsAlive).ToList();

            lock (this) dead.ForEach(ac => _delegates.Remove(ac));
        }

        #endregion
    }
}