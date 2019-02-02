using System;
using ExpressionBuilder.Fluent;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    /// <summary>The interception policy.</summary>
    public class InterceptionPolicy : IPolicy
    {
        public Func<IBuildContext, IRightable[], IRightable> Interceptor { get; set; }

        //public object Param { get; set; }

        //public TType GetParam<TType>() => Param is TType type ? type : default;
    }
}