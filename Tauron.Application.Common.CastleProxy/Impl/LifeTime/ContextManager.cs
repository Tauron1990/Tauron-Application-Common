using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Tauron.Application.Ioc;
using Tauron.Application.Ioc.LifeTime;

namespace Tauron.Application.Common.CastleProxy.Impl.LifeTime
{
    [PublicAPI]
    public static class ContextManager
    {
        private class WeakContext : IWeakReference
        {
            private readonly WeakReference _holder;

            public WeakContext(object owner) => _holder = new WeakReference(owner);

            public ObjectContext Context { get; set; }

            public object Owner => _holder.Target;

            public bool IsAlive => _holder.IsAlive;

        }
        private static readonly Dictionary<string, WeakContext> AspectContexts = Initialize();
        private static WeakReferenceCollection<WeakContext> _weakContexts;

        public static ObjectContext FindContext(object target, string contextName)
        {
            Argument.NotNull(target, nameof(target));
            Argument.NotNull(contextName, nameof(contextName));

            if (contextName != null)
            {
                var weakHolder = AspectContexts[contextName];
                var context = weakHolder.Context;
                return context;
            }

            if (target is IContextHolder holder) return holder.Context;

            var temp = _weakContexts.FirstOrDefault(con => ReferenceEquals(target, con.Owner));
            if (temp == null) throw new InvalidOperationException();

            return temp.Context;
        }

        public static ObjectContext GetContext(string name, [NotNull] object owner)
        {
            Argument.NotNull(name, nameof(name));
            if (AspectContexts.TryGetValue(name, out var context))
                return context.Context;

            var tempContext = new ObjectContext();
            AddContext(name, tempContext, owner);

            return tempContext;
        }

        public static ObjectContext GetContext([NotNull] object target)
        {
            Argument.NotNull(target, nameof(target));

            var context = new ObjectContext();
            _weakContexts.Add(new WeakContext(target) {Context = context});
            return context;
        }

        private static void AddContext([NotNull] string name, ObjectContext context, object owner) => AspectContexts[name] = new WeakContext(owner) {Context = context};

        private static void CleanContexts()
        {
            lock (AspectContexts)
            {
                var reference =
                    AspectContexts.Where(pair => !pair.Value.IsAlive).Select(pair => pair.Key).ToArray();
                foreach (var equalableWeakReference in reference) AspectContexts.Remove(equalableWeakReference);
            }
        }

        private static Dictionary<string, WeakContext> Initialize()
        {
            WeakCleanUp.RegisterAction(CleanContexts);
            _weakContexts = new WeakReferenceCollection<WeakContext>();
            return new Dictionary<string, WeakContext>();
        }
    }
}