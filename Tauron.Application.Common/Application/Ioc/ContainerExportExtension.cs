using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using JetBrains.Annotations;

namespace Tauron.Application.Ioc
{
    [PublicAPI]
    public static class ContainerExportExtension
    {
        public static void AddApplicationPath(this IContainer continer) => AddPath(continer, AppDomain.CurrentDomain.BaseDirectory, "*.*", SearchOption.AllDirectories, false);

        public static void AddAssembly([NotNull]this IContainer container, [NotNull] Assembly assembly)
        {
            Argument.NotNull(assembly, nameof(assembly));
            
            Register(container, resolver => resolver.AddAssembly(assembly));
        }

        public static void AddPath([NotNull]this IContainer container, [NotNull] string path, [NotNull] string searchpattern, SearchOption option, bool discoverChanges)
        {
            Argument.NotNull((object)path, nameof(path));
            Argument.NotNull(searchpattern, nameof(searchpattern));

            Register(container, resolver => resolver.AddPath(path, searchpattern, option, discoverChanges));
        }

        public static void AddPath([NotNull]this IContainer container, [NotNull] string path) => AddPath(container, path, "*.dll");
        public static void AddPath([NotNull]this IContainer container, [NotNull] string path, string searchpattern)
            => AddPath(container, path, searchpattern, SearchOption.TopDirectoryOnly);
        public static void AddPath([NotNull]this IContainer container, [NotNull] string path, [NotNull] string searchpattern, SearchOption searchOption) 
            => AddPath(container, path, searchpattern, searchOption, false);

        public static void AddProvider([NotNull]this IContainer container, [NotNull] ExportProvider provider)
        {
            Argument.NotNull(provider, nameof(provider));
            Register(container, resolver => resolver.AddProvider(provider));
        }

        public static void AddTypes([NotNull]this IContainer container, [NotNull] IEnumerable<Type> types)
        {
            Argument.NotNull(types, nameof(types));

            Register(container, resolver => resolver.AddTypes(types));
        }

        private static void Register(IContainer container, Action<ExportResolver> addAction)
        {
            Argument.NotNull(container, nameof(container));

            ExportResolver resolver = new ExportResolver();
            addAction(resolver);
            container.Register(resolver);
        }
    }
}