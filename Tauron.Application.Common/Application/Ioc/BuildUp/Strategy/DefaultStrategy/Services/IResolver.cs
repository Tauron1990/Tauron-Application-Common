using JetBrains.Annotations;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DefaultStrategy
{
    public interface IResolver
    {
        [CanBeNull]
        object Create(ErrorTracer errorTracer);
    }
}