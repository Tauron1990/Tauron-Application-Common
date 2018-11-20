using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.Application.Ioc.BuildUp.Exports.DefaultExports;
using Tauron.Application.Ioc.BuildUp.Strategy;
using Tauron.Application.Ioc.Components;

namespace Tauron.Application.Ioc.BuildUp
{
    [PublicAPI]
    public sealed class BuildEngine
    {
        public BuildEngine([NotNull] IContainer container, [NotNull] ExportProviderRegistry providerRegistry, [NotNull] ComponentRegistry componentRegistry)
        {
            Argument.NotNull(container, nameof(container));
            Argument.NotNull(providerRegistry, nameof(providerRegistry));
            Argument.NotNull(componentRegistry, nameof(componentRegistry));

            _container = container;
            _componentRegistry = componentRegistry;
            _factory = componentRegistry.GetAll<IExportFactory>()
                    .First(fac => fac.TechnologyName == AopConstants.DefaultExportFactoryName)
                    .SafeCast<DefaultExportFactory>();

            Pipeline = new Pipeline(componentRegistry);
            RebuildManager = new RebuildManager();
            providerRegistry.ExportsChanged += ExportsChanged;
        }
        
        private readonly DefaultExportFactory _factory;
        private readonly IContainer _container;
        private readonly ComponentRegistry _componentRegistry;
        
        public RebuildManager RebuildManager { get; }

        public Pipeline Pipeline { get; set; }

        public object BuildUp([NotNull] IExport export, [CanBeNull] string contractName, [NotNull] ErrorTracer tracer, [CanBeNull] BuildParameter[] buildParameters)
        {
            Argument.NotNull(export, nameof(export));
            Argument.NotNull(tracer, nameof(tracer));

            lock (export)
            {
                try
                {
                    tracer.Phase = "Begin Building Up";
                    var context = new DefaultBuildContext(export, BuildMode.Resolve, _container, contractName,
                        tracer, buildParameters, _componentRegistry.GetAll<IResolverExtension>().ToArray());
                    var buildObject = new BuildObject(export.ImportMetadata, context.Metadata, buildParameters);
                    Pipeline.Build(context);
                    if (tracer.Exceptional) return null;
                    buildObject.Instance = context.Target;
                    if (!export.ExternalInfo.External && !export.ExternalInfo.HandlesLiftime)
                        RebuildManager.AddBuild(buildObject);

                    return context.Target;
                }
                catch (Exception e)
                {
                    tracer.Exceptional = true;
                    tracer.Exception = e;
                    return null;
                }
            }
        }
        
        public object BuildUp([NotNull] object toBuild, [NotNull] ErrorTracer errorTracer, [NotNull] BuildParameter[] buildParameters)
        {
            Argument.NotNull(toBuild, nameof(toBuild));
            Argument.NotNull(errorTracer, nameof(errorTracer));
            Argument.NotNull(buildParameters, nameof(buildParameters));

            lock (toBuild)
            {
                try
                {
                    errorTracer.Phase = "Begin Building Up";
                    var context = new DefaultBuildContext(
                        _factory.CreateAnonymosWithTarget(toBuild.GetType(), toBuild),
                        BuildMode.BuildUpObject,
                        _container,
                        toBuild.GetType().Name, errorTracer,
                        buildParameters, _componentRegistry.GetAll<IResolverExtension>().ToArray());
                    Pipeline.Build(context);
                    return context.Target;
                }
                catch (Exception e)
                {
                    errorTracer.Exceptional = true;
                    errorTracer.Exception = e;
                    return null;
                }
            }
        }
        
        internal object BuildUp([NotNull] Type type, [CanBeNull] object[] constructorArguments, ErrorTracer errorTracer, [CanBeNull] BuildParameter[] buildParameters)
        {
            Argument.NotNull(type, nameof(type));

            lock (type)
            {
                errorTracer.Phase = "Begin Building Up";
                try
                {
                    var context = new DefaultBuildContext(
                        _factory.CreateAnonymos(type, constructorArguments),
                        BuildMode.BuildUpObject,
                        _container,
                        type.Name, errorTracer,
                        buildParameters, _componentRegistry.GetAll<IResolverExtension>().ToArray());
                    Pipeline.Build(context);
                    return context.Target;
                }
                catch (Exception e)
                {
                    errorTracer.Exceptional = true;
                    errorTracer.Exception = e;
                    return null;
                }
            }
        }

        private void BuildUp(BuildObject build, ErrorTracer errorTracer, BuildParameter[] buildParameters)
        {
            lock (build.Export)
            {
                var context = new DefaultBuildContext(build, _container, errorTracer, buildParameters);
                build.Instance = context.Target;
                Pipeline.Build(context);
            }
        }
        
        private void ExportsChanged([NotNull] object sender, [NotNull] ExportChangedEventArgs e)
        {
            Argument.NotNull(sender, nameof(sender));
            Argument.NotNull(e , nameof(e));

            var parts = RebuildManager.GetAffectedParts(e.Added, e.Removed);

            var errors = new List<ErrorTracer>();

            foreach (var buildObject in parts)
            {
                var errorTracer = new ErrorTracer();
                BuildUp(buildObject, errorTracer, buildObject.BuildParameters);

                if (errorTracer.Exceptional)
                    errors.Add(errorTracer);
            }

            if (errors.Count != 0)
                throw new AggregateException(errors.Select(err => new BuildUpException(err)));
        }
    }
}