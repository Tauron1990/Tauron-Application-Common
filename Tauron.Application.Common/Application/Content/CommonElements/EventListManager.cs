using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;

namespace Tauron.Application
{
    [Serializable]
    [DebuggerNonUserCode]
    [PublicAPI]
    public abstract class EventListManager : BaseObject
    {
        [NonSerialized]
        private Dictionary<string, Delegate> _handlers;

        public bool UseDispatcher { get; set; }

        [NotNull]
        private Dictionary<string, Delegate> Handlers => _handlers ?? (_handlers = new Dictionary<string, Delegate>());

        protected virtual void AddEvent([NotNull] string name, [NotNull] Delegate handler)
        {
            Argument.NotNull(name, nameof(name));
            Argument.NotNull(handler, nameof(handler));

            if (Handlers.ContainsKey(name)) Handlers[name] = Delegate.Combine(Handlers[name], handler);
            else Handlers[name] = handler;
        }

        protected virtual void InvokeEvent([NotNull] string name, [NotNull] params object[] args)
        {
            Argument.NotNull(name, nameof(name));
            Argument.NotNull(args, nameof(args));

            if (!Handlers.ContainsKey(name)) return;

            if (UseDispatcher)
                UiSynchronize.Synchronize.Invoke(() => Handlers[name].DynamicInvoke(args));
            else
                Handlers[name].DynamicInvoke(args);
        }

        protected virtual void RemoveEvent([NotNull] string name, [NotNull] Delegate handler)
        {
            Argument.NotNull(name, nameof(name));
            Argument.NotNull(handler, nameof(handler));

            if (!Handlers.ContainsKey(name)) return;

            var del = Handlers[name];
            del = Delegate.Remove(del, handler);

            if (del == null) Handlers.Remove(name);
            else Handlers[name] = del;
        }
    }
}