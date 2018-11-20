using JetBrains.Annotations;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    public interface IResolver
    {
        [CanBeNull]
        object Create(ErrorTracer errorTracer);
    }
}