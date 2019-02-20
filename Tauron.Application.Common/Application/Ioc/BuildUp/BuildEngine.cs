using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        private readonly ComponentRegistry _componentRegistry;
        private readonly ExportRegistry _exportRegistry;

        private readonly Dictionary<ExportMetadata, Func<object, object>> _factorys = new Dictionary<ExportMetadata, Func<object, object>>();

        public RebuildManager RebuildManager { get; }

        public Pipeline Pipeline { get; set; }

        public BuildEngine([NotNull] ExportProviderRegistry providerRegistry, [NotNull] ComponentRegistry componentRegistry, [NotNull] ExportRegistry exportRegistry)
        {
            Argument.NotNull(providerRegistry, nameof(providerRegistry));
            Argument.NotNull(componentRegistry, nameof(componentRegistry));
            Argument.NotNull(exportRegistry, nameof(exportRegistry));
            
            _componentRegistry = componentRegistry;
            _exportRegistry = exportRegistry;
            _factory = componentRegistry.GetAll<IExportFactory>()
                .First(fac => fac.TechnologyName == AopConstants.DefaultExportFactoryName)
                .SafeCast<DefaultExportFactory>();

            Pipeline = new Pipeline(componentRegistry);
            RebuildManager = componentRegistry.Get<RebuildManager>();
            providerRegistry.ExportsChanged += ExportsChanged;
        }

        public Expression AddExpressionsFor(CompilationUnit unit, ExportMetadata data, ErrorTracer errorTracer, BuildParameter[] parameters)
        {
            try
            {
                unit.VariableNamer.AddLevel();
                errorTracer.Phase = "Begin Building Up";

                var context = new DefaultBuildContext(data, this, errorTracer, parameters,
                    _componentRegistry.GetAll<IResolverExtension>().ToArray(), unit);
                Pipeline.Build(context);

                return Expression.Variable(typeof(object), unit.TargetName);
            }
            finally
            {
                unit.VariableNamer.RemoveLevel();
            }
        }

        public Expression CreateOperationBlock(ExportMetadata data, ErrorTracer errorTracer, params BuildParameter[] parameters)
        {
            try
            {
                errorTracer.Phase = "Begin Building Up";
                var unit = new CompilationUnit();

                var context = new DefaultBuildContext(data, this, errorTracer, parameters,
                    _componentRegistry.GetAll<IResolverExtension>().ToArray(), unit);
                Pipeline.Build(context);

                return Expression.Block(unit.Variables.Values, unit.Expressions);
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
                        //tracer.Phase = "Begin Building Up";
                        //var context = new DefaultBuildContext(meta2, _container, tracer, buildParameters,
                        //    _componentRegistry.GetAll<IResolverExtension>().ToArray(), new CompilationUnit(s => new FunctionCompilionTarget(s), null));
                        //Pipeline.Build(context);

                        var expression = Expression.Lambda(CreateOperationBlock(meta2, tracer, buildParameters));
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

        public ExportMetadata FindExport(Type interfaceType, string name, ErrorTracer errorTrancer, bool optional)
        {
            return optional 
                ? _exportRegistry.FindOptional(interfaceType, name, errorTrancer) 
                : _exportRegistry.FindSingle(interfaceType, name, errorTrancer);
        }
    }
}