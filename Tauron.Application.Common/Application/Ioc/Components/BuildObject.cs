using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;

namespace Tauron.Application.Ioc.Components
{
    [PublicAPI]
    public class BuildObject : IWeakReference
    {
        public BuildObject([NotNull] IEnumerable<ImportMetadata> imports, [NotNull] ExportMetadata targetExport, [CanBeNull] BuildParameter[] buildParameters)
        {
            Argument.NotNull(imports, nameof(imports));
            Argument.NotNull(targetExport, nameof(targetExport));

            Metadata = targetExport;
            _imports = imports.ToArray();
            Export = targetExport.Export;
            BuildParameters = buildParameters;
        }
        public ImportMetadata[] GetImports() => (ImportMetadata[]) _imports.Clone();

        private readonly ImportMetadata[] _imports;

        private WeakReference _instance;

        public IExport Export { get; private set; }

        public object Instance
        {
            get => _instance.Target;
            set => _instance = new WeakReference(value);
        }

        public ExportMetadata Metadata { get; set; }

        public bool IsAlive => _instance.IsAlive;

        [CanBeNull]
        public BuildParameter[] BuildParameters { get; set; }
    }
}