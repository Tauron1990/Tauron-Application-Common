namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    public class NodifyBuildCompledStrategy : StrategyBase
    {
        public override void OnPostBuild(IBuildContext context)
        {
            if (!(context.Target is INotifyBuildCompled notify)) return;

            context.ErrorTracer.Phase = "Notify Build Compled for " + context.Metadata;
            notify.BuildCompled();
        }
    }
}