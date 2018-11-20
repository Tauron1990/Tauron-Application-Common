using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tauron.Application.Ioc.BuildUp.Exports;

namespace Tauron.Application.Ioc
{
    [Serializable]
    public class AggregareExportProvider : ExportProvider
    {

        private readonly List<ExportResolver.AssemblyExportProvider> _assemblys =
            new List<ExportResolver.AssemblyExportProvider>();

        private IExportFactory _factory;

        public override bool BroadcastChanges => true;

        public override string Technology => AopConstants.DefaultExportFactoryName;

        public void Add(Assembly assembly)
        {
            Argument.NotNull(assembly, nameof(assembly));

            var export = new ExportResolver.AssemblyExportProvider(assembly);
            _assemblys.Add(export);

            if (_factory != null)
            {
                OnExportsChanged(
                    new ExportChangedEventArgs(
                        export.CreateExports(_factory).SelectMany(ex => ex.Item1.ExportMetadata),
                        Enumerable.Empty<ExportMetadata>()));
            }
        }

        public void AddRange(IEnumerable<Assembly> addAssemblys)
        {
            Argument.NotNull(addAssemblys, nameof(addAssemblys));

            var temp = addAssemblys.Where(asm => asm != null).Select(asm => new ExportResolver.AssemblyExportProvider(asm)).ToArray();
            if(temp.Length == 0) return;

            _assemblys.AddRange(temp);

            if (_factory != null)
            {
                OnExportsChanged(
                    new ExportChangedEventArgs(
                        temp.SelectMany(prov => prov.CreateExports(_factory))
                            .SelectMany(ex => ex.Item1.ExportMetadata),
                        Enumerable.Empty<ExportMetadata>()));
            }
        }

        public override IEnumerable<Tuple<IExport, int>> CreateExports(IExportFactory factory)
        {
            _factory = factory;
            return _assemblys.SelectMany(prov => prov.CreateExports(factory));
        }

        public void Remove(Assembly assembly)
        {
            Argument.NotNull(assembly, nameof(assembly));

            var export = new ExportResolver.AssemblyExportProvider(assembly);

            var index = _assemblys.IndexOf(export);
            if (index != -1)
            {
                export = _assemblys[index];
                if (!_assemblys.Remove(export)) export = null;
            }
            else
                export = null;

            if (_factory != null && export != null)
            {
                OnExportsChanged(
                    new ExportChangedEventArgs(
                        Enumerable.Empty<ExportMetadata>(),
                        export.CreateExports(_factory).SelectMany(ex => ex.Item1.ExportMetadata)));
            }
        }

        public void RemoveRange(IEnumerable<Assembly> removeAssemblys)
        {
            Argument.NotNull(removeAssemblys, nameof(removeAssemblys));

            var temp = removeAssemblys.Where(asm => asm != null).Select(asm => new ExportResolver.AssemblyExportProvider(asm)).ToList();
            if(temp.Count == 0) return;

            var indiexes = temp.Select(provider => _assemblys.IndexOf(provider)).Where(index => index != -1).ToList();
            temp.Clear();

            foreach (var index in indiexes)
            {
                temp.Add(_assemblys[index]);
                _assemblys.RemoveAt(index);
            }

            if (_factory != null)
            {
                OnExportsChanged(
                    new ExportChangedEventArgs(
                        Enumerable.Empty<ExportMetadata>(),
                        temp.SelectMany(prov => prov.CreateExports(_factory))
                            .SelectMany(ex => ex.Item1.ExportMetadata)));
            }
        }
    }
}