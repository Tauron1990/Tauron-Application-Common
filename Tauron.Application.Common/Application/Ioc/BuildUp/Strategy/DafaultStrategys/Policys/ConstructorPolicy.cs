using System;
using ExpressionBuilder.Fluent;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    public class ConstructorPolicy : IPolicy
    { 
        public Func<IBuildContext, IRightable> Constructor { get; set; }

    }
}