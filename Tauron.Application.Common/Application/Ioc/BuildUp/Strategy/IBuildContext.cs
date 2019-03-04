using System;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.Application.Ioc.BuildUp.Strategy.DefaultStrategy;

namespace Tauron.Application.Ioc.BuildUp.Strategy
{
    [PublicAPI]
    public interface IBuildContext
    {
        IResolverExtension[] ResolverExtensions { get; }

        bool BuildCompled { get; set; }

        [NotNull]
        IContainer Container { get; }

        [NotNull]
        Type ExportType { get; set; }

        [NotNull]
        ExportMetadata Metadata { get; }

        BuildMode Mode { get; }

        [NotNull]
        PolicyList Policys { get; }

        [CanBeNull]
        object Target { get; set; }

        [NotNull]
        ErrorTracer ErrorTracer { get; }

        [CanBeNull]
        BuildParameter[] Parameters { get; }

        FactoryCacheEntry CacheEntry { get; set; }
    }
}