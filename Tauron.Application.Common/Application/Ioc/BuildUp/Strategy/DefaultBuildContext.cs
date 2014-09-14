#region

using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.Application.Ioc.Components;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Strategy
{
    /// <summary>The default build context.</summary>
    public sealed class DefaultBuildContext : IBuildContext
    {
        #region Constructors and Destructors

        [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1027:TabsMustNotBeUsed",
            Justification = "Reviewed. Suppression is OK here.")]
        public DefaultBuildContext([NotNull] IExport targetExport, BuildMode mode, [NotNull] IContainer container, 
            [NotNull] string contractName, [NotNull] ErrorTracer errorTracer, [CanBeNull] BuildParameter[] parameters, 
            [NotNull] IResolverExtension[] resolverExtensions)
        {
            Contract.Requires<ArgumentNullException>(targetExport != null, "targetExport");
            Contract.Requires<ArgumentNullException>(container != null, "container");
            Contract.Requires<ArgumentNullException>(errorTracer != null, "errorTracer");
            Contract.Requires<ArgumentNullException>(resolverExtensions != null, "resolverExtensions");

            Metadata = targetExport.GetNamedExportMetadata(contractName);
            errorTracer.Export = Metadata.ToString();
            ExportType = targetExport.ImplementType;
            Target = null;
            BuildCompled = false;
            Policys = new PolicyList();
            Mode = mode;
            Container = container;
            ErrorTracer = errorTracer;
            Parameters = parameters;
            ResolverExtensions = resolverExtensions;
        }

        /// <summary>
        ///     Initialisiert eine neue Instanz der <see cref="DefaultBuildContext" /> Klasse.
        /// </summary>
        /// <param name="buildObject">
        ///     The build object.
        /// </param>
        /// <param name="container">
        ///     The container.
        /// </param>
        /// <param name="errorTracer"></param>
        /// <param name="parameters"></param>
        public DefaultBuildContext([NotNull] BuildObject buildObject, [NotNull] IContainer container, [NotNull] ErrorTracer errorTracer, [CanBeNull] BuildParameter[] parameters)
        {
            Contract.Requires<ArgumentNullException>(buildObject != null, "buildObject");
            Contract.Requires<ArgumentNullException>(container != null, "container");
            Contract.Requires<ArgumentNullException>(errorTracer != null, "errorTracer");

            Metadata = buildObject.Metadata;
            errorTracer.Export = Metadata.ToString();
            Mode = BuildMode.BuildUpObject;
            Policys = new PolicyList();
            Target = buildObject.Instance;
            ExportType = Metadata.Export.ImplementType;
            BuildCompled = false;
            Container = container;
            ErrorTracer = errorTracer;
            Parameters = parameters;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets a value indicating whether build compled.</summary>
        /// <value>The build compled.</value>
        public bool BuildCompled { get; set; }

        /// <summary>Gets the container.</summary>
        /// <value>The container.</value>
        public IContainer Container { get; private set; }

        /// <summary>Gets or sets the export type.</summary>
        /// <value>The export type.</value>
        public Type ExportType { get; set; }

        /// <summary>Gets the metadata.</summary>
        /// <value>The metadata.</value>
        public ExportMetadata Metadata { get; private set; }

        /// <summary>Gets the mode.</summary>
        /// <value>The mode.</value>
        public BuildMode Mode { get; private set; }

        /// <summary>Gets the policys.</summary>
        /// <value>The policys.</value>
        public PolicyList Policys { get; private set; }

        /// <summary>Gets or sets the target.</summary>
        /// <value>The target.</value>
        public object Target { get; set; }

        public ErrorTracer ErrorTracer { get; private set; }
        public BuildParameter[] Parameters { get; private set; }
        public IResolverExtension[] ResolverExtensions { get; set; }

        #endregion

        public override string ToString()
        {
            return Metadata.ToString();
        }
    }
}