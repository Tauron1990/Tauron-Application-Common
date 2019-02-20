using System.Linq.Expressions;
using JetBrains.Annotations;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    public interface IResolver
    {
        [NotNull]
        Expression Create(ErrorTracer errorTracer, CompilationUnit unit);
    }
}