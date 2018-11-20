using JetBrains.Annotations;
using Tauron.Application.Views.Core;

namespace Tauron.Application.Views
{
    [PublicAPI]
    public sealed class ExportNameHelper : ISortableViewExportMetadata
    {
        private readonly object _dependencyObject;

        public ExportNameHelper([NotNull] string name, [NotNull] object dependencyObject)
        {
            _dependencyObject = Argument.NotNull(dependencyObject, nameof(dependencyObject));
            Name = Argument.NotNull(name, nameof(name));
            Order = ViewManager.GetSortOrder(dependencyObject);
        }

        public string Name { get; }

        public int Order { get; }

        [NotNull]
        public object GetMeta() => this;

        [NotNull]
        public object GetValue() => _dependencyObject;
    }
}