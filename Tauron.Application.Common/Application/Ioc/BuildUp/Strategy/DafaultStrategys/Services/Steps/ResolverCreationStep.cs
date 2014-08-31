using Tauron.Application.SimpleWorkflow;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys.Steps
{
    public class ResolverCreationStep : InjectorStep
    {
        public override StepId Id
        {
            get { return StepIds.ResolverCreation; }
        }
    }
}