using System;
using Tauron.Application.Ioc.LifeTime;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    public sealed class LiftimeStrategy : StrategyBase
    {
        public override void OnPerpare(IBuildContext context)
        {
            if (!context.CanHandleLiftime()) return;
            context.ErrorTracer.Phase = "Reciving Liftime Informations for " + context.Metadata;

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

            policy.LifetimeContext = (ILifetimeContext) Activator.CreateInstance(policy.LiftimeType);
            policy.LifetimeContext.SetValue(context.Target);
        }
    }
}