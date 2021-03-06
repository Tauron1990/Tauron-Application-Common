﻿using System.Diagnostics;
using JetBrains.Annotations;
using Tauron.Application.SimpleWorkflow;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DefaultStrategy.Steps
{
    public abstract class InjectorStep : IStep<InjectorContext>
    {
        public abstract string ErrorMessage { get; }

        public abstract StepId Id { get; }

        [DebuggerStepThrough]
        public virtual StepId OnExecute([NotNull] InjectorContext context) => StepId.None;

        [DebuggerStepThrough]
        public virtual StepId NextElement([NotNull] InjectorContext context) => StepId.LoopEnd;

        [DebuggerStepThrough]
        public virtual void OnExecuteFinish([NotNull] InjectorContext context)
        {
        }

        [DebuggerStepThrough]
        public override string ToString() => Id.ToString();
    }
}