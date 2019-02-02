using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;

namespace Tauron.Application.Ioc.BuildUp.Strategy
{
    public sealed class DefaultBuildContext : IBuildContext
    {
        public override string ToString() => Metadata.ToString();
        
        [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1027:TabsMustNotBeUsed",
            Justification = "Reviewed. Suppression is OK here.")]
        public DefaultBuildContext([NotNull] ExportMetadata targetExport,[NotNull] IContainer container, 
            [NotNull] ErrorTracer errorTracer, [CanBeNull] BuildParameter[] parameters, [CanBeNull] IResolverExtension[] resolverExtensions)
        {
            errorTracer.Export = Metadata.ToString();
            ExportType = targetExport.Export.ImplementType;
            Metadata = Argument.NotNull(targetExport, nameof(targetExport));
            Policys = new PolicyList();
            Container = Argument.NotNull(container, nameof(container));
            ErrorTracer = Argument.NotNull(errorTracer, nameof(errorTracer));
            Parameters = parameters;
            ResolverExtensions = resolverExtensions;
        }
        
        public IContainer Container { get; private set; }

        public Type ExportType { get; set; }

        public ExportMetadata Metadata { get; private set; }

        public PolicyList Policys { get; private set; }
        
        public ErrorTracer ErrorTracer { get; private set; }

        public BuildParameter[] Parameters { get; private set; }

        public CompilationUnit CompilationUnit { get; } = new CompilationUnit();

        public IResolverExtension[] ResolverExtensions { get; set; }
    }
}