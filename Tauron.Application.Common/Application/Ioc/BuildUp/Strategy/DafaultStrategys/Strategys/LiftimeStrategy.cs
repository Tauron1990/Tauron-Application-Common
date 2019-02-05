using ExpressionBuilder;

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

            var unit = context.CompilationUnit;

            string lifetimecontext = unit.LifeTimeContext;
            string inputName = unit.Input;

           unit
                .AddCode(CodeLine.CreateIf(Condition.Compare(inputName, Operation.Null()))
                    .Then(CodeLine.Assign(lifetimecontext, Operation.CreateInstance(policy.LiftimeType)),
                        Operation.Invoke(lifetimecontext, "SetValue", Operation.Variable(unit.TargetName))));
        }
    }
}