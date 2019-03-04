using System;
using System.Collections.Generic;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DefaultStrategy
{
    public sealed class FactoryCacheEntry
    {
        public List<Action<IBuildContext>> PrepareActions { get; } = new List<Action<IBuildContext>>();

        public List<Action<IBuildContext>> InstanceActions { get; } = new List<Action<IBuildContext>>();

        public List<Action<IBuildContext>> BuildActions { get; } = new List<Action<IBuildContext>>();

        public List<Action<IBuildContext>> PostBuildAction { get; } = new List<Action<IBuildContext>>();
    }
}