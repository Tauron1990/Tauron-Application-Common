using System;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.SimpleWorkflow
{
    [PublicAPI]
    public class SimpleCondition<TContext> : ICondition<TContext>
    {
        [CanBeNull]
        public Func<TContext, IStep<TContext>, bool> Guard { get; set; }

        public StepId Target { get; set; }

        public SimpleCondition()
        {
            Target = StepId.None;
        }

        public StepId Select(IStep<TContext> lastStep, TContext context)
        {
            if (Guard == null || Guard(context, lastStep)) return Target;

            return StepId.None;
        }

        public override string ToString()
        {
            return "Target: " + Target;
        }
    }
}