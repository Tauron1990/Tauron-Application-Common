using Tauron.Application.Ioc.Components;
using Tauron.Application.Ioc.LifeTime;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DefaultStrategy
{
    public class CacheStrategy : StrategyBase
    {
        private ICache _cache;

        public override void Initialize(ComponentRegistry components) => _cache = components.Get<ICache>();

        public override void OnPrepare(IBuildContext context)
        {
            if (!context.IsBuildExport()) return;

            context.ErrorTracer.Phase = "Receiving Build (" + context.Metadata + ") from Cache";

            var life = _cache.GetContext(context.Metadata);
            if (context.Metadata.Lifetime.GetCustomAttribute<CacheCreationProcessAttribute>()?.Cache == true)
            {
                context.CacheEntry = new FactoryCacheEntry();

                context.CacheEntry.PrepareActions.Add(c => RunPrep(c, _cache.GetContext(context.Metadata)));
            }

            RunPrep(context, life);
        }

        private static void RunPrep(IBuildContext context, ILifetimeContext life)
        {
            var value = life?.GetValue();
            if (value == null) return;

            context.Target = value;
            context.BuildCompled = true;
        }

        public override void OnPostBuild(IBuildContext context)
        {
            if (!context.CanCache()) return;

            context.ErrorTracer.Phase = "Saving Build (" + context.Metadata + ") to Cache";

            var policy = context.Policys.Get<LifetimeTimePolicy>();

            if (policy == null) return;

            _cache.Add(policy.LifetimeContext, context.Metadata, policy.ShareLiftime);
        }
    }
}