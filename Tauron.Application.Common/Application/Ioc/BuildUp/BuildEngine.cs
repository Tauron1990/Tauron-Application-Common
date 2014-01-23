// The file BuildEngine.cs is part of Tauron.Application.Common.
// 
// CoreEngine is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// CoreEngine is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//  
// You should have received a copy of the GNU General Public License
//  along with Tauron.Application.Common If not, see <http://www.gnu.org/licenses/>.

#region

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BuildEngine.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The build engine.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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
        private readonly IContainer container;

        private readonly Pipeline pipeline;

        private readonly RebuildManager rebuildManager;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BuildEngine" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="BuildEngine" /> Klasse.
        ///     Initializes a new instance of the <see cref="BuildEngine" /> class.
        /// </summary>
        /// <param name="container">
        ///     The container.
        /// </param>
        /// <param name="providerRegistry">
        ///     The provider registry.
        /// </param>
        /// <param name="componentRegistry">
        ///     The component registry.
        /// </param>
        public BuildEngine(
            IContainer container,
            ExportProviderRegistry providerRegistry,
            ComponentRegistry componentRegistry)
        {
            Contract.Requires<ArgumentNullException>(container != null, "container");
            Contract.Requires<ArgumentNullException>(providerRegistry != null, "providerRegistry");
            Contract.Requires<ArgumentNullException>(componentRegistry != null, "componentRegistry");

            this.container = container;
            _factory =
                componentRegistry.GetAll<IExportFactory>()
                                 .First(fac => fac.TechnologyName == AopConstants.DefaultExportFactoryName)
                                 .CastObj<DefaultExportFactory>();
            pipeline = new Pipeline(componentRegistry);
            rebuildManager = new RebuildManager();
            providerRegistry.ExportsChanged += ExportsChanged;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the pipeline.</summary>
        /// <value>The pipeline.</value>
        public Pipeline Pipeline
        {
            get
            {
                Contract.Ensures(Contract.Result<Pipeline>() != null);

                return pipeline;
            }
        }

        /// <summary>Gets the rebuild manager.</summary>
        /// <value>The rebuild manager.</value>
        public RebuildManager RebuildManager
        {
            get
            {
                Contract.Ensures(Contract.Result<RebuildManager>() != null);

                return rebuildManager;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The build up.
        /// </summary>
        /// <param name="export">
        ///     The export.
        /// </param>
        /// <param name="contractName">
        ///     The contract name.
        /// </param>
        /// <param name="tracer"></param>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        public object BuildUp(IExport export, string contractName, ErrorTracer tracer)
        {
            Contract.Requires<ArgumentNullException>(export != null, "export");

            lock (export)
            {
                try
                {
                    tracer.Phase = "Begin Building Up";
                    var context = new DefaultBuildContext(export, BuildMode.Resolve, container, contractName, tracer);
                    var buildObject = new BuildObject(export.ImportMetadata, context.Metadata);
                    Pipeline.Build(context);
                    if (tracer.Exceptional) return null;
                    buildObject.Instance = context.Target;
                    if (!export.ExternalInfo.External && !export.ExternalInfo.HandlesLiftime) 
                        RebuildManager.AddBuild(buildObject);

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

        /// <summary>
        ///     The build up.
        /// </summary>
        /// <param name="toBuild"></param>
        /// <param name="export">
        ///     The export.
        /// </param>
        /// <param name="errorTracer"></param>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        public object BuildUp(object toBuild, ErrorTracer errorTracer)
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
                        container,
                        toBuild.GetType().ToString(), errorTracer);
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

        /// <summary>
        ///     The build up.
        /// </summary>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <param name="constructorArguments">
        ///     The constructor arguments.
        /// </param>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        internal object BuildUp(Type type, object[] constructorArguments, ErrorTracer errorTracer)
        {
            Contract.Requires<ArgumentNullException>(type != null, "type");

            errorTracer.Phase = "Begin Building Up";
            try
            {
                var context = new DefaultBuildContext(
                    _factory.CreateAnonymos(type, constructorArguments),
                    BuildMode.BuildUpObject,
                    container,
                    type.ToString(), errorTracer);
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

        private void BuildUp(BuildObject build, ErrorTracer errorTracer)
        {
            lock (build.Export)
            {
                var context = new DefaultBuildContext(build, container, errorTracer);
                build.Instance = context.Target;
                Pipeline.Build(context);
            }
        }

        /// <summary>
        ///     The exports changed.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void ExportsChanged(object sender, ExportChangedEventArgs e)
        {
            Contract.Requires<ArgumentNullException>(sender != null, "sender");

            IEnumerable<BuildObject> parts = RebuildManager.GetAffectedParts(e.Added, e.Removed);

            var errors = new List<ErrorTracer>();

            foreach (BuildObject buildObject in parts)
            {
                var errorTracer = new ErrorTracer();
                BuildUp(buildObject, errorTracer);

                if(errorTracer.Exceptional)
                    errors.Add(errorTracer);
            }

            if(errors.Count != 0)
                throw new AggregateException(errors.Select(err => new BuildUpException(err)));
        }

        #endregion
    }
}