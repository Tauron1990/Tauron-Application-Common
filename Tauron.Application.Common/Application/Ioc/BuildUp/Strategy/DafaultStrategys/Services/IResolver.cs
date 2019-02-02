using ExpressionBuilder.Fluent;
using JetBrains.Annotations;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    public interface IResolver
    {
        [CanBeNull]
        IRightable Create(ErrorTracer errorTracer);
    }
}