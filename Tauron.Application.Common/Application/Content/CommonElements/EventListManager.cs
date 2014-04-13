#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The event list manager.</summary>
    [Serializable]
    [DebuggerNonUserCode]
    public class EventListManager : BaseObject
    {
        #region Fields

        /// <summary>The _handlers.</summary>
        [NonSerialized] private Dictionary<string, Delegate> _handlers;

        #endregion

        #region Properties

        public bool UseDispatcher { get; set; }

        /// <summary>Gets the handlers.</summary>
        /// <value>The handlers.</value>
        [NotNull]
        private Dictionary<string, Delegate> Handlers
        {
            get { return _handlers ?? (_handlers = new Dictionary<string, Delegate>()); }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The add event.
        /// </summary>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <param name="handler">
        ///     The handler.
        /// </param>
        protected virtual void AddEvent([NotNull] string name, [NotNull] Delegate handler)
        {
            Contract.Requires<ArgumentNullException>(name != null, "name");
            Contract.Requires<ArgumentNullException>(handler != null, "handler");

            if (Handlers.ContainsKey(name)) Handlers[name] = Delegate.Combine(Handlers[name], handler);
            else Handlers[name] = handler;
        }

        /// <summary>
        ///     The invoke event.
        /// </summary>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <param name="args">
        ///     The args.
        /// </param>
        protected virtual void InvokeEvent([NotNull] string name, [NotNull] params object[] args)
        {
            Contract.Requires<ArgumentNullException>(name != null, "name");
            Contract.Requires<ArgumentNullException>(args != null, "args");

            if (!Handlers.ContainsKey(name)) return;

            if(UseDispatcher)
                UiSynchronize.Synchronize.Invoke(() => Handlers[name].DynamicInvoke(args));
            else
                Handlers[name].DynamicInvoke(args);
        }

        /// <summary>
        ///     The remove event.
        /// </summary>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <param name="handler">
        ///     The handler.
        /// </param>
        protected virtual void RemoveEvent([NotNull] string name, [NotNull] Delegate handler)
        {
            Contract.Requires<ArgumentNullException>(name != null, "name");
            Contract.Requires<ArgumentNullException>(handler != null, "handler");

            if (!Handlers.ContainsKey(name)) return;

            Delegate del = Handlers[name];
            del = Delegate.Remove(del, handler);

            if (del == null) Handlers.Remove(name);
            else Handlers[name] = del;
        }

        #endregion
    }
}