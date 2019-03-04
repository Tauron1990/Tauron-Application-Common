using Tauron.Application.SimpleWorkflow;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DefaultStrategy.Steps
{
    public class ResolverCreationStep : InjectorStep
    {
        public override string ErrorMessage { get; } = nameof(ResolverCreationStep);

        public override StepId Id => StepIds.ResolverCreation;
    }
}