using System;
using Tauron.Application.Ioc.LifeTime;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DefaultStrategy
{
    public sealed class LifetimeTimePolicy : IPolicy
    {
        public ILifetimeContext LifetimeContext { get; set; }

        public Type LiftimeType { get; set; }

        public bool ShareLiftime { get; set; }

    }
}