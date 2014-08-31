using Tauron.Application.SimpleWorkflow;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys.Steps
{
    public abstract class InjectorStep : IStep<InjectorContext>
    {
        public abstract StepId Id { get; }

        public virtual StepId OnExecute([NotNull] InjectorContext context)
        {
            return StepId.None;
        }

        public virtual StepId NextElement([NotNull] InjectorContext context)
        {
            return StepId.LoopEnd;
        }

        public virtual void OnExecuteFinish([NotNull] InjectorContext context)
        {
        }

        public override string ToString()
        {
            return Id.ToString();
        }
    }
}