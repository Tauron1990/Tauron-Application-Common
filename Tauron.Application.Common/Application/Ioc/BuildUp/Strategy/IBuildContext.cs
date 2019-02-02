using System;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;

namespace Tauron.Application.Ioc.BuildUp.Strategy
{
    [PublicAPI]
    public interface IBuildContext
    {
        CompilationUnit CompilationUnit { get; }

        IResolverExtension[] ResolverExtensions { get; }
        
        [NotNull]
        IContainer Container { get; }

        [NotNull]
        Type ExportType { get; set; }

        [NotNull]
        ExportMetadata Metadata { get; }
        
        [NotNull]
        PolicyList Policys { get; }
        
        [NotNull]
        ErrorTracer ErrorTracer { get; }

        [CanBeNull]
        BuildParameter[] Parameters { get; }
        
    }
}