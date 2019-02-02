using System;
using Tauron.Application.Ioc.LifeTime;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    public sealed class LifetimeTimePolicy : IPolicy
    {
        public Type LiftimeType { get; set; }

        public bool ShareLiftime { get; set; }

    }
}