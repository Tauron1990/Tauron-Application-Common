using Tauron.Application.Ioc.Components;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    public class CacheStrategy : StrategyBase
    {
        private ICache _cache;

        public override void Initialize(ComponentRegistry components) => _cache = components.Get<ICache>();

        public override void OnPerpare(IBuildContext context)
        {
            if (!context.IsBuildExport()) return;

            context.ErrorTracer.Phase = "Reciving Build (" + context.Metadata + ") from Cache";

            var life = _cache.GetContext(context.Metadata);

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