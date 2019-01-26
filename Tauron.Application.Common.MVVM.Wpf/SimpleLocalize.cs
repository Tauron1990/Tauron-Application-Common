using System;
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
        public SimpleLocalize([NotNull] string name) => Name = name;

        public SimpleLocalize() { }

        [CanBeNull]
        public string Name { get; set; }

        [Obsolete(nameof(ResourceManagerProvider))]
        public static void Register([NotNull] ResourceManager manager, [NotNull] Assembly key) => ResourceManagerProvider.Register(manager, key);

        [Obsolete(nameof(ResourceManagerProvider))]
        public static void Remove([NotNull] Assembly key) => ResourceManagerProvider.Remove(key);

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (string.IsNullOrWhiteSpace(Name)) return string.Empty;

            var provider = serviceProvider.GetService(typeof(IRootObjectProvider)) as IRootObjectProvider;
            if (provider?.RootObject == null) return Name; // "IRootObjectProvider oder das RootObject existieren nicht!";

            return ResourceManagerProvider.FindResource(Name, provider.RootObject.GetType().Assembly, false) ?? Name;
        }
    }
}