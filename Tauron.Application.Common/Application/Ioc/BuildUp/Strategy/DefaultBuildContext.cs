using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.Application.Ioc.BuildUp.Strategy.DefaultStrategy;
using Tauron.Application.Ioc.Components;

namespace Tauron.Application.Ioc.BuildUp.Strategy
{
    public sealed class DefaultBuildContext : IBuildContext
    {
        public override string ToString() => Metadata.ToString();
        
        [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1027:TabsMustNotBeUsed",
            Justification = "Reviewed. Suppression is OK here.")]
        public DefaultBuildContext([NotNull] IExport targetExport, BuildMode mode, [NotNull] IContainer container, [CanBeNull] string contractName,
            [NotNull] ErrorTracer errorTracer, [CanBeNull] BuildParameter[] parameters, [CanBeNull] IResolverExtension[] resolverExtensions)
        {
            if (!Enum.IsDefined(typeof(BuildMode), mode)) throw new InvalidEnumArgumentException(nameof(mode), (int) mode, typeof(BuildMode));
            Metadata = Argument.NotNull(targetExport, nameof(targetExport)).GetNamedExportMetadata(contractName);
            errorTracer.Export = Metadata.ToString();
            ExportType = targetExport.ImplementType;
            Target = null;
            BuildCompled = false;
            Policys = new PolicyList();
            Mode = mode;
            Container = Argument.NotNull(container, nameof(container));
            ErrorTracer = Argument.NotNull(errorTracer, nameof(errorTracer));
            Parameters = parameters;
            ResolverExtensions = resolverExtensions;
        }

        public DefaultBuildContext([NotNull] BuildObject buildObject, [NotNull] IContainer container, [NotNull] ErrorTracer errorTracer, [CanBeNull] BuildParameter[] parameters)
        {
            Metadata = Argument.NotNull(buildObject, nameof(buildObject)).Metadata;
            Mode = BuildMode.BuildUpObject;
            Policys = new PolicyList();
            Target = buildObject.Instance;
            ExportType = Metadata.Export.ImplementType;
            BuildCompled = false;
            Container = Argument.NotNull(container, nameof(container));
            ErrorTracer = Argument.NotNull(errorTracer, nameof(errorTracer));
            Parameters = parameters;
            errorTracer.Export = Metadata.ToString();
        }

        public bool BuildCompled { get; set; }

        public IContainer Container { get; private set; }

        public Type ExportType { get; set; }

        public ExportMetadata Metadata { get; private set; }

        public BuildMode Mode { get; private set; }

        public PolicyList Policys { get; private set; }

        public object Target { get; set; }

        public ErrorTracer ErrorTracer { get; private set; }
        public BuildParameter[] Parameters { get; private set; }
        public FactoryCacheEntry CacheEntry { get; set; }
        public IResolverExtension[] ResolverExtensions { get; set; }
    }
}