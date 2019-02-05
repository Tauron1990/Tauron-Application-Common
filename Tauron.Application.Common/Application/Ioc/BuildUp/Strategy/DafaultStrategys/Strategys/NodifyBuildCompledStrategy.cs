using ExpressionBuilder;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    public class NodifyBuildCompledStrategy : StrategyBase
    {
        public override void OnPostBuild(IBuildContext context)
        {
            if (!typeof(INotifyBuildCompled).IsAssignableFrom(context.ExportType)) return;

            context.ErrorTracer.Phase = "Notify Build Compled for " + context.Metadata;

            context.CompilationUnit.AddAndPush(Operation.Invoke(context.CompilationUnit.TargetName, nameof(INotifyBuildCompled.BuildCompled)));
        }
    }
}