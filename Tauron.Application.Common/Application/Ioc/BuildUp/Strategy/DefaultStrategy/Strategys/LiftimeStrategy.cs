using Tauron.Application.Ioc.LifeTime;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DefaultStrategy
{
    public sealed class LiftimeStrategy : StrategyBase
    {
        public override void OnPrepare(IBuildContext context)
        {
            if (!context.CanHandleLiftime()) return;
            context.ErrorTracer.Phase = "Reciving Liftime Information for " + context.Metadata;

            context.Policys.Add(
                new LifetimeTimePolicy
                {
                    LiftimeType = context.Metadata.Lifetime,
                    ShareLiftime = context.Metadata.Export.ShareLifetime
                });
        }

        public override void OnPostBuild(IBuildContext context)
        {
            var policy = context.Policys.Get<LifetimeTimePolicy>();
            if (policy == null) return;

            context.ErrorTracer.Phase = "Setting up Liftime for " + context.Metadata;

            var life = (ILifetimeContext) policy.LiftimeType.FastCreateInstance();
            policy.LifetimeContext = life;
            policy.LifetimeContext.SetValue(context.Target);
            
            context.CacheEntry?.PostBuildAction.Add(c => SetLifeTime(c, life));
        }

        private static void SetLifeTime(IBuildContext context, ILifetimeContext life) => life.SetValue(context.Target);
    }
}