namespace Tauron.Application.Ioc.BuildUp.Strategy.DefaultStrategy
{
    public class NodifyBuildCompledStrategy : StrategyBase
    {
        public override void OnPostBuild(IBuildContext context)
        {
            context.CacheEntry?.PostBuildAction.Add(Notify);

            Notify(context);
        }

        private static void Notify(IBuildContext context)
        {
            if (!(context.Target is INotifyBuildCompled notify)) return;

            context.ErrorTracer.Phase = "Notify Build Compled for " + context.Metadata;
            notify.BuildCompled();
        }
    }
}