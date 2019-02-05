using ExpressionBuilder.Fluent;
using JetBrains.Annotations;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    public interface IResolver
    {
        [NotNull]
        IRightable Create(ErrorTracer errorTracer, CompilationUnit unit);
    }
}