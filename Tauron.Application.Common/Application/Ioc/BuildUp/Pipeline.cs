using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Strategy;
using Tauron.Application.Ioc.Components;

namespace Tauron.Application.Ioc.BuildUp
{
    public sealed class Pipeline
    {
        private readonly ComponentRegistry _registry;

        public Pipeline([NotNull] ComponentRegistry registry)
        {
            Argument.NotNull(registry, nameof(registry));
            _registry = registry;
        }

        public void Build(IBuildContext context)
        {
            try
            {
                IEnumerable<IStrategy> strategies = _registry.GetAll<IStrategy>().ToArray();
                if (Invoke(strategies, strategy => strategy.OnPerpare(context), context)) return;

                if (Invoke(strategies, strategy => strategy.OnCreateInstance(context), context)) return;

                if (Invoke(strategies.Reverse(), strategy => strategy.OnBuild(context), context)) return;

                Invoke(strategies.Reverse(), strategy => strategy.OnPostBuild(context), context);
            }
            catch (Exception e)
            {
                context.ErrorTracer.Exceptional = true;
                context.ErrorTracer.Exception = e;
            }
        }
        
        private static bool Invoke([NotNull] IEnumerable<IStrategy> strategies, [NotNull] Action<IStrategy> invoker, [NotNull] IBuildContext context)
        {
            foreach (var strategy in strategies)
            {
                if (context.ErrorTracer.Exceptional) return true;

                invoker(strategy);
            }

            return false;
        }
    }
}