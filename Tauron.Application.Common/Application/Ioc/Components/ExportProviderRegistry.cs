using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Tauron.Application.Ioc.Components
{
    [PublicAPI]
    public sealed class ExportProviderRegistry : IDisposable
    {
        private readonly List<ExportProvider> _providers;

        public ExportProviderRegistry() => _providers = new List<ExportProvider>();
        public event EventHandler<ExportChangedEventArgs> ExportsChanged;

        private void OnExportsChanged([NotNull] object sender, ExportChangedEventArgs e)
        {
            Argument.NotNull(sender, nameof(sender));
            var handler = ExportsChanged;
            handler?.Invoke(sender, e);
        }

        public void Dispose()
        {
            foreach (var exportProvider in _providers)
                if (exportProvider is IDisposable dipo)
                    dipo.Dispose();
        }

        public void Add([NotNull] ExportProvider provider)
        {
            Argument.NotNull(provider, nameof(provider));
            lock (_providers)
            {
                _providers.Add(provider);
                provider.ExportsChanged += OnExportsChanged;
            }
        }
    }
}