// The file InstanceResolver.cs is part of Tauron.Application.Common.
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
// <copyright file="InstanceResolver.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The export resolver.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.Application.Ioc.BuildUp.Exports.DefaultExports;
using Tauron.Application.Ioc.Components;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Ioc
{
    /// <summary>The export resolver.</summary>
    [PublicAPI]
    public sealed class ExportResolver
    {
        #region Fields

        /// <summary>The _providers.</summary>
        private readonly List<ExportProvider> _providers;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExportResolver" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="ExportResolver" /> Klasse.
        ///     Initializes a new instance of the <see cref="ExportResolver" /> class.
        /// </summary>
        public ExportResolver()
        {
            _providers = new List<ExportProvider>();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The add assembly.
        /// </summary>
        /// <param name="assembly">
        ///     The assembly.
        /// </param>
        public void AddAssembly(Assembly assembly)
        {
            Contract.Requires<ArgumentNullException>(assembly != null, "assembly");

            AddProvider(new AssemblyExportProvider(assembly));
        }

        /// <summary>
        ///     The add path.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <param name="searchpattern">
        ///     The searchpattern.
        /// </param>
        /// <param name="option">
        ///     The option.
        /// </param>
        /// <param name="discoverChanges">
        ///     The discover changes.
        /// </param>
        public void AddPath(string path, string searchpattern, SearchOption option, bool discoverChanges)
        {
            Contract.Requires<ArgumentNullException>(path != null, "path");
            Contract.Requires<ArgumentNullException>(searchpattern != null, "searchpattern");

            if (path == string.Empty) path = AppDomain.CurrentDomain.BaseDirectory;

            path = path.GetFullPath();

            if (!path.ExisDirectory()) return;

            AddProvider(new PathExportProvider(path, searchpattern, option, discoverChanges));
        }

        /// <summary>
        ///     The add path.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        public void AddPath(string path)
        {
            Contract.Requires<ArgumentNullException>(path != null, "path");

            AddPath(path, "*.dll");
        }

        /// <summary>
        ///     The add path.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <param name="searchpattern">
        ///     The searchpattern.
        /// </param>
        public void AddPath(string path, string searchpattern)
        {
            Contract.Requires<ArgumentNullException>(path != null, "path");
            Contract.Requires<ArgumentNullException>(searchpattern != null, "searchpattern");

            AddPath(path, searchpattern, SearchOption.TopDirectoryOnly);
        }

        /// <summary>
        ///     The add path.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <param name="searchpattern">
        ///     The searchpattern.
        /// </param>
        /// <param name="searchOption">
        ///     The search option.
        /// </param>
        public void AddPath(string path, string searchpattern, SearchOption searchOption)
        {
            Contract.Requires<ArgumentNullException>(path != null, "path");
            Contract.Requires<ArgumentNullException>(searchpattern != null, "searchpattern");

            AddPath(path, searchpattern, searchOption, false);
        }

        /// <summary>
        ///     The add provider.
        /// </summary>
        /// <param name="provider">
        ///     The provider.
        /// </param>
        public void AddProvider(ExportProvider provider)
        {
            Contract.Requires<ArgumentNullException>(provider != null, "provider");

            _providers.Add(provider);
        }

        /// <summary>
        ///     The add types.
        /// </summary>
        /// <param name="types">
        ///     The types.
        /// </param>
        public void AddTypes(IEnumerable<Type> types)
        {
            Contract.Requires<ArgumentNullException>(types != null, "types");

            AddProvider(new TypeExportProvider(types));
        }

        /// <summary>
        ///     The fill.
        /// </summary>
        /// <param name="componentRegistry">
        ///     The component registry.
        /// </param>
        /// <param name="exportRegistry">
        ///     The export registry.
        /// </param>
        /// <param name="exportProviderRegistry">
        ///     The export provider registry.
        /// </param>
        public void Fill(
            ComponentRegistry componentRegistry,
            ExportRegistry exportRegistry,
            ExportProviderRegistry exportProviderRegistry)
        {
            Contract.Requires<ArgumentNullException>(componentRegistry != null, "componentRegistry");
            Contract.Requires<ArgumentNullException>(exportRegistry != null, "exportRegistry");
            Contract.Requires<ArgumentNullException>(exportProviderRegistry != null, "exportProviderRegistry");

            var factorys = new Dictionary<string, IExportFactory>();
            foreach (IExportFactory factory in componentRegistry.GetAll<IExportFactory>()) factorys[factory.TechnologyName] = factory;

            foreach (ExportProvider exportProvider in _providers)
            {
                foreach (Tuple<IExport, int> export in exportProvider.CreateExports(factorys[exportProvider.Technology])) 
                    exportRegistry.Register(export.Item1, export.Item2);

                if (exportProvider.BroadcastChanges) exportProviderRegistry.Add(exportProvider);
            }
        }

        #endregion

        /// <summary>The assembly export provider.</summary>
        internal sealed class AssemblyExportProvider : ExportProvider, IEquatable<AssemblyExportProvider>
        {
            #region Fields

            /// <summary>The asm.</summary>
            internal readonly Assembly Asm;

            /// <summary>The _provider.</summary>
            private TypeExportProvider _provider;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="AssemblyExportProvider" /> class.
            ///     Initialisiert eine neue Instanz der <see cref="AssemblyExportProvider" /> Klasse.
            ///     Initializes a new instance of the <see cref="AssemblyExportProvider" /> class.
            /// </summary>
            /// <param name="asm">
            ///     The asm.
            /// </param>
            public AssemblyExportProvider(Assembly asm)
            {
                Contract.Requires<ArgumentNullException>(asm != null, "asm");

                Asm = asm;
            }

            #endregion

            #region Public Properties

            /// <summary>Gets the technology.</summary>
            /// <value>The technology.</value>
            public override string Technology
            {
                get { return AopConstants.DefaultExportFactoryName; }
            }

            #endregion

            #region Public Methods and Operators

            /// <summary>
            ///     The equals.
            /// </summary>
            /// <param name="other">
            ///     The other.
            /// </param>
            /// <returns>
            ///     The <see cref="bool" />.
            /// </returns>
            public bool Equals(AssemblyExportProvider other)
            {
                if (ReferenceEquals(null, other)) return false;

                return ReferenceEquals(this, other) || Equals(Asm.FullName, other.Asm.FullName);
            }

            /// <summary>The ==.</summary>
            /// <param name="left">The left.</param>
            /// <param name="right">The right.</param>
            /// <returns></returns>
            public static bool operator ==(AssemblyExportProvider left, AssemblyExportProvider right)
            {
                return Equals(left, right);
            }

            /// <summary>The !=.</summary>
            /// <param name="left">The left.</param>
            /// <param name="right">The right.</param>
            /// <returns></returns>
            public static bool operator !=(AssemblyExportProvider left, AssemblyExportProvider right)
            {
                return !Equals(left, right);
            }

            /// <summary>
            ///     The create exports.
            /// </summary>
            /// <param name="factory">
            ///     The factory.
            /// </param>
            /// <returns>
            ///     The <see cref="IEnumerable" />.
            /// </returns>
            public override IEnumerable<Tuple<IExport, int>> CreateExports(IExportFactory factory)
            {
                if (_provider == null) _provider = new TypeExportProvider(Asm.GetTypes());

                return _provider.CreateExports(factory);
            }

            /// <summary>
            ///     The equals.
            /// </summary>
            /// <param name="obj">
            ///     The obj.
            /// </param>
            /// <returns>
            ///     The <see cref="bool" />.
            /// </returns>
            public override bool Equals(object obj)
            {
                var prov = obj as AssemblyExportProvider;
                if (ReferenceEquals(prov, null)) return false;

                return Equals(prov);
            }

            /// <summary>The get hash code.</summary>
            /// <returns>
            ///     The <see cref="int" />.
            /// </returns>
            public override int GetHashCode()
            {
                return Asm != null ? Asm.GetHashCode() : 0;
            }

            #endregion
        }

        /// <summary>The path export provider.</summary>
        private sealed class PathExportProvider : ExportProvider, IDisposable
        {
            #region Fields

            /// <summary>The _discover changes.</summary>
            private readonly bool _discoverChanges;

            /// <summary>The _files.</summary>
            private readonly List<string> _files;

            /// <summary>The _option.</summary>
            private readonly SearchOption _option;

            /// <summary>The _path.</summary>
            private readonly string _path;

            /// <summary>The _searchpattern.</summary>
            private readonly string _searchpattern;

            /// <summary>The _factory.</summary>
            private IExportFactory _factory;

            /// <summary>The _providers.</summary>
            private List<AssemblyExportProvider> _providers;

            /// <summary>The _watcher.</summary>
            private FileSystemWatcher _watcher;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="PathExportProvider" /> class.
            ///     Initialisiert eine neue Instanz der <see cref="PathExportProvider" /> Klasse.
            ///     Initializes a new instance of the <see cref="PathExportProvider" /> class.
            /// </summary>
            /// <param name="path">
            ///     The path.
            /// </param>
            /// <param name="searchpattern">
            ///     The searchpattern.
            /// </param>
            /// <param name="option">
            ///     The option.
            /// </param>
            /// <param name="discoverChanges">
            ///     The discover changes.
            /// </param>
            public PathExportProvider(string path, string searchpattern, SearchOption option, bool discoverChanges)
            {
                Contract.Requires<ArgumentNullException>(path != null, "path");
                Contract.Requires<ArgumentNullException>(searchpattern != null, "searchpattern");

                _path = path;
                _searchpattern = searchpattern;
                _option = option;
                _discoverChanges = discoverChanges;
                _files = new List<string>(Directory.EnumerateFiles(path, searchpattern, option));
            }

            /// <summary>
            ///     Finalizes an instance of the <see cref="PathExportProvider" /> class.
            ///     Finalisiert eine Instanz der <see cref="PathExportProvider" /> Klasse.
            /// </summary>
            ~PathExportProvider()
            {
                Dispose();
            }

            #endregion

            #region Public Properties

            /// <summary>Gets a value indicating whether broadcast changes.</summary>
            /// <value>The broadcast changes.</value>
            public override bool BroadcastChanges
            {
                get { return _discoverChanges; }
            }

            /// <summary>Gets the technology.</summary>
            /// <value>The technology.</value>
            public override string Technology
            {
                get { return AopConstants.DefaultExportFactoryName; }
            }

            #endregion

            #region Public Methods and Operators

            /// <summary>The dispose.</summary>
            public void Dispose()
            {
                if (_watcher != null) _watcher.Dispose();

                GC.SuppressFinalize(this);
            }

            /// <summary>
            ///     The create exports.
            /// </summary>
            /// <param name="factory">
            ///     The factory.
            /// </param>
            /// <returns>
            ///     The <see cref="IEnumerable" />.
            /// </returns>
            public override IEnumerable<Tuple<IExport, int>> CreateExports(IExportFactory factory)
            {
                _factory = factory;

                if (_providers == null)
                {
                    _providers = new List<AssemblyExportProvider>();
                    foreach (string file in _files)
                    {
                        AssemblyExportProvider exportProvider = null;
                        try
                        {
                            exportProvider = new AssemblyExportProvider(Assembly.LoadFile(file));
                        }
                        catch (FileLoadException)
                        {
                        }
                        catch (BadImageFormatException)
                        {
                        }

                        if (exportProvider == null) continue;

                        _providers.Add(exportProvider);
                    }
                }

                if (_discoverChanges)
                {
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
                }

                return _providers.SelectMany(pro => pro.CreateExports(factory));
            }

            #endregion

            #region Methods

            /// <summary>
            ///     The created.
            /// </summary>
            /// <param name="sender">
            ///     The sender.
            /// </param>
            /// <param name="e">
            ///     The e.
            /// </param>
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

            /// <summary>
            ///     The deleted.
            /// </summary>
            /// <param name="sender">
            ///     The sender.
            /// </param>
            /// <param name="e">
            ///     The e.
            /// </param>
            private void Deleted(object sender, FileSystemEventArgs e)
            {
                if (!Path.HasExtension(e.FullPath) || _providers == null) return;

                try
                {
                    var pro = new AssemblyExportProvider(Assembly.LoadFrom(e.FullPath));
                    int index = _providers.IndexOf(pro);
                    if (index == -1) return;

                    pro = _providers[index];

                    Contract.Assume(pro != null);

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

            #endregion
        }

        /// <summary>The type export provider.</summary>
        private sealed class TypeExportProvider : ExportProvider
        {
            #region Fields

            /// <summary>The _types.</summary>
            private readonly IEnumerable<Type> _types;

            /// <summary>The _exports.</summary>
            private Tuple<IExport, int>[] _exports;

            private int _level;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="TypeExportProvider" /> class.
            ///     Initialisiert eine neue Instanz der <see cref="TypeExportProvider" /> Klasse.
            ///     Initializes a new instance of the <see cref="TypeExportProvider" /> class.
            /// </summary>
            /// <param name="types">
            ///     The types.
            /// </param>
            public TypeExportProvider([NotNull] IEnumerable<Type> types)
            {
                Contract.Requires<ArgumentNullException>(types != null, "types");

                _types = types;
                _level = 0;
            }

            public TypeExportProvider([NotNull] IEnumerable<Type> types, int level)
            {
                Contract.Requires<ArgumentNullException>(types != null, "types");

                _types = types;
                _level = level;
            }

            #endregion

            #region Public Properties

            /// <summary>Gets the technology.</summary>
            /// <value>The technology.</value>
            public override string Technology
            {
                get { return AopConstants.DefaultExportFactoryName; }
            }

            #endregion

            #region Public Methods and Operators

            /// <summary>
            ///     The create exports.
            /// </summary>
            /// <param name="factory">
            ///     The factory.
            /// </param>
            /// <returns>
            ///     The <see cref="IEnumerable" />.
            /// </returns>
            public override IEnumerable<Tuple<IExport, int>> CreateExports(IExportFactory factory)
            {
                var fac = (DefaultExportFactory) factory;

                if (_exports != null) return _exports;

                var exports = new List<Tuple<IExport, int>>(_types.Count());

                foreach (Type type in _types)
                {
                    int currentLevel = _level;

                    IExport ex1 = fac.Create(type, ref currentLevel);
                    if (ex1 != null) exports.Add(Tuple.Create(ex1, currentLevel));

                    exports.AddRange(
                        type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                            .Select(methodInfo => fac.CreateMethodExport(methodInfo, ref currentLevel))
                            .Where(ex2 => ex2 != null)
                            .Select(exp => Tuple.Create(exp, currentLevel)));
                }

                this._exports = exports.ToArray();
                return this._exports;
            }

            #endregion
        }
    }
}