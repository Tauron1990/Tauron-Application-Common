using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.Application.Ioc.BuildUp.Exports.DefaultExports;
using Tauron.Application.Ioc.Components;

namespace Tauron.Application.Ioc
{
    [PublicAPI]
    [Serializable]
    public sealed class ExportResolver
    {
        private readonly List<ExportProvider> _providers;

        public ExportResolver() => _providers = new List<ExportProvider>();

        [Serializable]
        internal sealed class AssemblyExportProvider : ExportProvider, IEquatable<AssemblyExportProvider>
        {
            public AssemblyExportProvider([NotNull] Assembly asm)
            {
                Argument.NotNull(asm, nameof(asm));
                Asm = asm;
            }
            public override string Technology => AopConstants.DefaultExportFactoryName;

            internal readonly Assembly Asm;

            private TypeExportProvider _provider;

            public bool Equals(AssemblyExportProvider other)
            {
                if (ReferenceEquals(null, other)) return false;

                return ReferenceEquals(this, other) || Equals(Asm.FullName, other.Asm.FullName);
            }

            public static bool operator ==(AssemblyExportProvider left, AssemblyExportProvider right) => Equals(left, right);

            public static bool operator !=(AssemblyExportProvider left, AssemblyExportProvider right) => !Equals(left, right);

            public override IEnumerable<Tuple<IExport, int>> CreateExports(IExportFactory factory)
            {
                if (_provider == null) _provider = new TypeExportProvider(Asm.GetTypes());

                return _provider.CreateExports(factory);
            }

            public override bool Equals(object obj)
            {
                var prov = obj as AssemblyExportProvider;
                return !ReferenceEquals(prov, null) && Equals(prov);
            }

            public override int GetHashCode() => Asm != null ? Asm.GetHashCode() : 0;
        }

        [Serializable]
        private sealed class PathExportProvider : ExportProvider, IDisposable
        {
            private readonly bool _discoverChanges;
            private readonly List<string> _files;
            private readonly SearchOption _option;
            private readonly string _path;
            private readonly string _searchpattern;
            private IExportFactory _factory;
            private List<AssemblyExportProvider> _providers;
            private FileSystemWatcher _watcher;

            public PathExportProvider([NotNull] string path, [NotNull] string searchpattern, SearchOption option, bool discoverChanges)
            {
                Argument.NotNull(path, nameof(path));
                Argument.NotNull(searchpattern, nameof(searchpattern));

                _path = path;
                _searchpattern = searchpattern;
                _option = option;
                _discoverChanges = discoverChanges;
                _files = new List<string>(Directory.EnumerateFiles(path, searchpattern, option));
            }

            ~PathExportProvider() => Dispose();

            public override bool BroadcastChanges => _discoverChanges;

            public override string Technology => AopConstants.DefaultExportFactoryName;

            public void Dispose()
            {
                _watcher?.Dispose();
                _watcher = null;

                GC.SuppressFinalize(this);
            }


            public override IEnumerable<Tuple<IExport, int>> CreateExports(IExportFactory factory)
            {
                _factory = factory;

                if (_providers == null)
                {
                    _providers = new List<AssemblyExportProvider>();
                    foreach (var file in _files)
                    {
                        AssemblyExportProvider exportProvider = null;
                        try
                        {
                            exportProvider = new AssemblyExportProvider(Assembly.LoadFile(file));
                        }
                        catch (FileLoadException) { }
                        catch (BadImageFormatException) { }

                        if (exportProvider == null) continue;

                        _providers.Add(exportProvider);
                    }
                }

                if (!_discoverChanges) return _providers.SelectMany(pro => pro.CreateExports(factory));

                _watcher = new FileSystemWatcher(_path, _searchpattern)
                {
                    EnableRaisingEvents = true,
                    IncludeSubdirectories =
                        _option
                        == SearchOption
                            .AllDirectories
                };
                _watcher.Created += Created;
                _watcher.Deleted += Deleted;

                return _providers.SelectMany(pro => pro.CreateExports(factory));
            }

            private void Created([NotNull] object sender, [NotNull] FileSystemEventArgs e)
            {
                if (!Path.HasExtension(e.FullPath) || _providers == null) return;

                try
                {
                    var pro = new AssemblyExportProvider(Assembly.LoadFrom(e.FullPath));

                    if (_providers.Contains(pro)) return;

                    _providers.Add(pro);

                    OnExportsChanged(
                        new ExportChangedEventArgs(
                            pro.CreateExports(_factory).SelectMany(exp => exp.Item1.ExportMetadata),
                            new ExportMetadata[0]));
                }
                catch (BadImageFormatException)
                {
                }
                catch (FileLoadException)
                {
                }
            }

            private void Deleted(object sender, FileSystemEventArgs e)
            {
                if (!Path.HasExtension(e.FullPath) || _providers == null) return;

                try
                {
                    var pro = new AssemblyExportProvider(Assembly.LoadFrom(e.FullPath));
                    var index = _providers.IndexOf(pro);
                    if (index == -1) return;

                    pro = _providers[index];

                    _providers.RemoveAt(index);
                    OnExportsChanged(
                        new ExportChangedEventArgs(
                            new ExportMetadata[0],
                            pro.CreateExports(_factory).SelectMany(exp => exp.Item1.ExportMetadata)));
                }
                catch (BadImageFormatException)
                {
                }
                catch (FileLoadException)
                {
                }
            }
        }
        
        [Serializable]
        private sealed class TypeExportProvider : ExportProvider
        {

            public override string Technology => AopConstants.DefaultExportFactoryName;


            public override IEnumerable<Tuple<IExport, int>> CreateExports(IExportFactory factory)
            {
                var fac = (DefaultExportFactory) factory;

                if (_exports != null) return _exports;

                var exports = new List<Tuple<IExport, int>>(_types.Count());

                foreach (var type in _types)
                {
                    var currentLevel = _level;

                    var ex1 = fac.Create(type, ref currentLevel);
                    if (ex1 != null) exports.Add(Tuple.Create(ex1, currentLevel));

                    exports.AddRange(
                        type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                            .Select(methodInfo => fac.CreateMethodExport(methodInfo, ref currentLevel))
                            .Where(ex2 => ex2 != null)
                            .Select(exp => Tuple.Create(exp, currentLevel)));
                }

                _exports = exports.ToArray();
                return _exports;
            }
            private readonly IEnumerable<Type> _types;
            
            private Tuple<IExport, int>[] _exports;

            private int _level;

            public TypeExportProvider([NotNull] IEnumerable<Type> types)
            {
                Argument.NotNull(types, nameof(types));

                _types = types;
                _level = 0;
            }

            public TypeExportProvider([NotNull] IEnumerable<Type> types, int level)
                : this(types) => _level = level;
        }

        public void AddAssembly([NotNull] Assembly assembly)
        {
            Argument.NotNull(assembly, nameof(assembly));
            AddProvider(new AssemblyExportProvider(assembly));
        }

        public void AddPath([NotNull] string path, [NotNull] string searchpattern, SearchOption option, bool discoverChanges)
        {
            Argument.NotNull((object)path, nameof(path));
            Argument.NotNull(searchpattern, nameof(searchpattern));
            if (path == string.Empty) path = AppDomain.CurrentDomain.BaseDirectory;

            path = path.GetFullPath();

            if (!path.ExisDirectory()) return;

            AddProvider(new PathExportProvider(path, searchpattern, option, discoverChanges));
        }

        public void AddPath([NotNull] string path) => AddPath(path, "*.dll");
        public void AddPath([NotNull] string path, string searchpattern) => AddPath(path, searchpattern, SearchOption.TopDirectoryOnly);
        public void AddPath([NotNull] string path, [NotNull] string searchpattern, SearchOption searchOption) => AddPath(path, searchpattern, searchOption, false);

        public void AddProvider([NotNull] ExportProvider provider)
        {
            Argument.NotNull(provider, nameof(provider));
            _providers.Add(provider);
        }

        public void AddTypes([NotNull] IEnumerable<Type> types)
        {
            Argument.NotNull(types, nameof(types));
            AddProvider(new TypeExportProvider(types));
        }

        public void Fill(
            [NotNull] ComponentRegistry componentRegistry,
            [NotNull] ExportRegistry exportRegistry,
            [NotNull] ExportProviderRegistry exportProviderRegistry)
        {
            Argument.NotNull(componentRegistry, nameof(componentRegistry));
            Argument.NotNull(exportRegistry, nameof(exportRegistry));
            Argument.NotNull(exportProviderRegistry, nameof(exportProviderRegistry));

            var factorys = new Dictionary<string, IExportFactory>();
            foreach (var factory in componentRegistry.GetAll<IExportFactory>()) factorys[factory.TechnologyName] = factory;

            foreach (var exportProvider in _providers)
            {
                foreach (var export in exportProvider.CreateExports(factorys[exportProvider.Technology])) exportRegistry.Register(export.Item1, export.Item2);

                if (exportProvider.BroadcastChanges) exportProviderRegistry.Add(exportProvider);
            }
        }
    }
}