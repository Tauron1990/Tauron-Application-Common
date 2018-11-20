using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;
using System.Windows.Markup;
using System.Xaml;
using JetBrains.Annotations;

namespace Tauron.Application
{
    [PublicAPI]
    public class SimpleLocalize : MarkupExtension
    {
        private static readonly Dictionary<Assembly, ResourceManager> Resources =
            new Dictionary<Assembly, ResourceManager>();

        public SimpleLocalize([NotNull] string name) => Name = name;

        public SimpleLocalize() { }

        [CanBeNull]
        public string Name { get; set; }

        public static void Register([NotNull] ResourceManager manager, [NotNull] Assembly key) => Resources[Argument.NotNull(key, nameof(key))] = Argument.NotNull(manager, nameof(manager));

        public static void Remove([NotNull] Assembly key) => Resources.Remove(key);

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (string.IsNullOrWhiteSpace(Name)) return string.Empty;

            var provider = serviceProvider.GetService(typeof(IRootObjectProvider)) as IRootObjectProvider;
            if (provider?.RootObject == null) return Name; // "IRootObjectProvider oder das RootObject existieren nicht!";

            return Resources.TryGetValue(provider.RootObject.GetType().Assembly, out var manager)
                ? manager.GetObject(Name)
                : Name;
        }
    }
}