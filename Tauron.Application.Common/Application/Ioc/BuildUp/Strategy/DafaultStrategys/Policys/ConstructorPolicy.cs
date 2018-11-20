using System;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    public class ConstructorPolicy : IPolicy
    { 
        public Func<IBuildContext, object> Constructor { get; set; }

    }
}