#region

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.Application.Ioc.BuildUp.Exports.DefaultExports;
using Tauron.Application.Ioc.BuildUp.Strategy;
using Tauron.Application.Ioc.Components;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Ioc.BuildUp
{
    /// <summary>The build engine.</summary>
    [PublicAPI]
    public sealed class BuildEngine
    {
        #region Fields

        /// <summary>The _factory.</summary>
        private readonly DefaultExportFactory _factory;

        /// <summary>The _container.</summary>
        private readonly IContainer _container;

        private readonly ComponentRegistry _componentRegistry;

        private readonly Pipeline _pipeline;

        private readonly RebuildManager _rebuildManager;

        #endregion

        #region Constructors and Destructors

        public BuildEngine([NotNull] IContainer container, [NotNull] ExportProviderRegistry providerRegistry, 
            [NotNull] ComponentRegistry componentRegistry)
        {
            Contract.Requires<ArgumentNullException>(container != null, "container");
            Contract.Requires<ArgumentNullException>(providerRegistry != null, "providerRegistry");
            Contract.Requires<ArgumentNullException>(componentRegistry != null, "componentRegistry");

            _container = container;
            _componentRegistry = componentRegistry;
            _factory =
                componentRegistry.GetAll<IExportFactory>()
                                 .First(fac => fac.TechnologyName == AopConstants.DefaultExportFactoryName)
                                 .CastObj<DefaultExportFactory>();
            _pipeline = new Pipeline(componentRegistry);
            _rebuildManager = new RebuildManager();
            providerRegistry.ExportsChanged += ExportsChanged;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the pipeline.</summary>
        /// <value>The pipeline.</value>
        [NotNull]
        public Pipeline Pipeline
        {
            get
            {
                Contract.Ensures(Contract.Result<Pipeline>() != null);

                return _pipeline;
            }
        }

        /// <summary>Gets the rebuild manager.</summary>
        /// <value>The rebuild manager.</value>
        [NotNull]
        public RebuildManager RebuildManager
        {
            get
            {
                Contract.Ensures(Contract.Result<RebuildManager>() != null);

                return _rebuildManager;
            }
        }

        #endregion

        #region Public Methods and Operators


        [CanBeNull]
        public object BuildUp([NotNull] IExport export, [NotNull] string contractName, [NotNull] ErrorTracer tracer, 
            [CanBeNull] BuildParameter[] buildParameters)
        {
            Contract.Requires<ArgumentNullException>(export != null, "export");

            lock (export)
            {
                try
                {
                    tracer.Phase = "Begin Building Up";
                    var context = new DefaultBuildContext(export, BuildMode.Resolve, _container, contractName, tracer,
                                                          buildParameters,
                                                          _componentRegistry.GetAll<IResolverExtension>().ToArray());
                    var buildObject = new BuildObject(export.ImportMetadata, context.Metadata, buildParameters);
                    Pipeline.Build(context);
                    if (tracer.Exceptional) return null;
                    buildObject.Instance = context.Target;
                    if (!export.ExternalInfo.External && !export.ExternalInfo.HandlesLiftime) RebuildManager.AddBuild(buildObject);

                    Contract.Assume(context.Target != null);
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


        [CanBeNull]
        public object BuildUp([NotNull] object toBuild, [NotNull] ErrorTracer errorTracer, 
            [CanBeNull] BuildParameter[] buildParameters)
        {
            Contract.Requires<ArgumentNullException>(errorTracer != null, "errorTracer");
            Contract.Requires<ArgumentNullException>(toBuild != null, "export");

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
                    Contract.Assume(context.Target != null);
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

        #endregion

        #region Methods

        [CanBeNull]
        internal object BuildUp([NotNull] Type type, [NotNull] object[] constructorArguments, 
            [NotNull] ErrorTracer errorTracer, [CanBeNull] BuildParameter[] buildParameters)
        {
            Contract.Requires<ArgumentNullException>(type != null, "type");

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
                Contract.Assume(context.Target != null);
                return context.Target;
            }
            catch (Exception e)
            {
                errorTracer.Exceptional = true;
                errorTracer.Exception = e;
                return null;
            }
        }

        private void BuildUp([NotNull] BuildObject build, [NotNull] ErrorTracer errorTracer, 
            [CanBeNull] BuildParameter[] buildParameters)
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
            Contract.Requires<ArgumentNullException>(sender != null, "sender");

            IEnumerable<BuildObject> parts = RebuildManager.GetAffectedParts(e.Added, e.Removed);

            var errors = new List<ErrorTracer>();

            foreach (BuildObject buildObject in parts)
            {
                var errorTracer = new ErrorTracer();
                BuildUp(buildObject, errorTracer, buildObject.BuildParameters);

                if (errorTracer.Exceptional) errors.Add(errorTracer);
            }

            if (errors.Count != 0) throw new AggregateException(errors.Select(err => new BuildUpException(err)));
        }

        #endregion
    }
}