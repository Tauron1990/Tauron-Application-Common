using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ExpressionBuilder.Fluent;
using FastExpressionCompiler;
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
        private readonly DefaultExportFactory _factory;
        private readonly IContainer _container;
        private readonly ComponentRegistry _componentRegistry;

        private readonly Dictionary<ExportMetadata, Func<object, object>> _factorys = new Dictionary<ExportMetadata, Func<object, object>>();

        public RebuildManager RebuildManager { get; }

        public Pipeline Pipeline { get; set; }

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
            RebuildManager = componentRegistry.Get<RebuildManager>();
            providerRegistry.ExportsChanged += ExportsChanged;
        }

        public IOperationBlock CreateOperationBlock(ExportMetadata data, ErrorTracer errorTracer, SubCompilitionUnit subUnit, params BuildParameter[] parameters)
        {
            try
            {
                errorTracer.Phase = "Begin Building Up";
                var context = new DefaultBuildContext(data, _container, errorTracer, parameters,
                    _componentRegistry.GetAll<IResolverExtension>().ToArray(), new CompilationUnit(s => new BlockFunctionTarget(s), subUnit.VariableNamer));
                Pipeline.Build(context);

                return (IOperationBlock)context.CompilationUnit.RealFunction.ToOperation();
            }
            catch (Exception e)
            {
                errorTracer.Exceptional = true;
                errorTracer.Exception = e;
                throw new BuildUpException(errorTracer);
            }
        }

        public Func<object, object> CreateDelegate(IExport export, string contractName, ErrorTracer tracer, BuildParameter[] buildParameters, bool isAnonymos, out ExportMetadata meta)
        {
            lock (_factorys)
            {
                meta = export.GetNamedExportMetadata(contractName);
                if (meta == null) throw new BuildUpException(tracer);

                return isAnonymos ? Creator(meta) : _factorys.GetOrAdd(meta, Creator);

                Func<object, object> Creator(ExportMetadata meta2)
                {
                    try
                    {
                        tracer.Phase = "Begin Building Up";
                        var context = new DefaultBuildContext(meta2, _container, tracer, buildParameters,
                            _componentRegistry.GetAll<IResolverExtension>().ToArray(), new CompilationUnit(s => new FunctionCompilionTarget(s), null));
                        Pipeline.Build(context);

                        var expression = (LambdaExpression)context.CompilationUnit.ToExpression();
                        return expression.CompileFast<Func<object, object>>();
                    }
                    catch (Exception e)
                    {
                        tracer.Exceptional = true;
                        tracer.Exception = e;
                        throw new BuildUpException(tracer);
                    }
                }
            }
        }

        public object BuildUp([NotNull] IExport export, [CanBeNull] string contractName, [NotNull] ErrorTracer tracer, [CanBeNull] BuildParameter[] buildParameters)
        {
            Argument.NotNull(export, nameof(export));
            Argument.NotNull(tracer, nameof(tracer));

            tracer.Phase = "Begin Building Up";
            var result = CreateDelegate(export, contractName, tracer, buildParameters, false, out _)(null);

            return result;
        }

        public object BuildUp([NotNull] object toBuild, [NotNull] ErrorTracer errorTracer, [NotNull] BuildParameter[] buildParameters, IExport relatedExport)
        {
            Argument.NotNull(toBuild, nameof(toBuild));
            Argument.NotNull(errorTracer, nameof(errorTracer));
            Argument.NotNull(buildParameters, nameof(buildParameters));

            errorTracer.Phase = "Begin Building Up";
            var export = relatedExport ?? _factory.CreateAnonymosWithTarget(toBuild.GetType(), toBuild);
            var result = CreateDelegate(export, string.Empty, errorTracer, buildParameters, relatedExport == null, out _)(toBuild);
            return result;
        }

        internal object BuildUp([NotNull] Type type, [CanBeNull] object[] constructorArguments, ErrorTracer errorTracer, [CanBeNull] BuildParameter[] buildParameters)
        {
            Argument.NotNull(type, nameof(type));

            errorTracer.Phase = "Begin Building Up";
            return CreateDelegate(_factory.CreateAnonymos(type, constructorArguments), string.Empty, errorTracer, buildParameters, true, out _)(null);
        }

        private void BuildUp(BuildObject build, ErrorTracer errorTracer, BuildParameter[] buildParameters)
        {
            object inst = build.Instance;
            if(inst == null) return;
            
            build.Instance = CreateDelegate(build.Export, build.Metadata.ContractName, errorTracer, build.BuildParameters, false, out _)(inst);
        }

        private void ExportsChanged([NotNull] object sender, [NotNull] ExportChangedEventArgs e)
        {
            Argument.NotNull(sender, nameof(sender));
            Argument.NotNull(e, nameof(e));

            var parts = RebuildManager.GetAffectedParts(e.Added, e.Removed);

            var errors = new List<ErrorTracer>();

            foreach (var buildObject in parts)
            {
                var errorTracer = new ErrorTracer();

                try
                {

                    BuildUp(buildObject, errorTracer, buildObject.BuildParameters);
                }
                catch (Exception exception)
                {
                    if (!(exception is BuildUpException))
                    {
                        errorTracer.Exceptional = true;
                        errorTracer.Exception = exception;
                    }
                }

                if (errorTracer.Exceptional)
                    errors.Add(errorTracer);
            }

            if (errors.Count != 0)
                throw new AggregateException(errors.Select(err => new BuildUpException(err)));
        }
    }
}