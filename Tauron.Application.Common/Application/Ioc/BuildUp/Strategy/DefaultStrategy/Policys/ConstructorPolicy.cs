using System;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DefaultStrategy
{
    public class ConstructorPolicy : IPolicy
    { 
        public Func<IBuildContext, object> Constructor { get; set; }

    }
}