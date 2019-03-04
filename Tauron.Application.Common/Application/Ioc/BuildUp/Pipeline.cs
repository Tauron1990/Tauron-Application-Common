using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Strategy;
using Tauron.Application.Ioc.Components;

namespace Tauron.Application.Ioc.BuildUp
{
    public sealed class Pipeline
    {
        private readonly ComponentRegistry _registry;
        private readonly ICache _cache;

        public Pipeline([NotNull] ComponentRegistry registry)
        {
            Argument.NotNull(registry, nameof(registry));
            _registry = registry;
            _cache = registry.Get<ICache>();
        }

        public void Build(IBuildContext context, bool forceNew)
        {
            try
            {
                var facEnt = _cache.Get(context.Metadata);
                if (facEnt != null && !forceNew && (context.Parameters?.Length ?? 0) == 0)
                {
                    if (Invoke(facEnt.PrepareActions, context)) return;

                    if (Invoke(facEnt.InstanceActions, context)) return;

                    if (Invoke(facEnt.BuildActions, context)) return;

                    Invoke(facEnt.PostBuildAction, context);
                }
                else
                {
                    IEnumerable<IStrategy> strategies = _registry.GetAll<IStrategy>().ToArray();
                    if (Invoke(strategies, strategy => strategy.OnPrepare(context), context)) return;

                    if (Invoke(strategies, strategy => strategy.OnCreateInstance(context), context)) return;

                    if (Invoke(strategies.Reverse(), strategy => strategy.OnBuild(context), context)) return;

                    Invoke(strategies.Reverse(), strategy => strategy.OnPostBuild(context), context);
                }
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
                if (context.BuildCompled || context.ErrorTracer.Exceptional) return true;

                invoker(strategy);
            }

            return false;
        }

        private static bool Invoke([NotNull] IEnumerable<Action<IBuildContext>> strategies, [NotNull] IBuildContext context)
        {
            foreach (var strategy in strategies)
            {
                if (context.BuildCompled || context.ErrorTracer.Exceptional) return true;

                strategy(context);
            }

            return false;
        }
    }
}