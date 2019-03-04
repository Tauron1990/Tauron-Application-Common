using System;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DefaultStrategy
{
    /// <summary>The interception policy.</summary>
    public class InterceptionPolicy : IPolicy
    {
        public Func<IBuildContext, object[], object> Interceptor { get; set; }

        public object Param { get; set; }

        public TType GetParam<TType>() => Param is TType type ? type : default;
    }
}