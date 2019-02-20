using System;
using System.Linq.Expressions;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    public class ConstructorPolicy : IPolicy
    { 
        public Func<IBuildContext, Expression> Constructor { get; set; }

    }
}