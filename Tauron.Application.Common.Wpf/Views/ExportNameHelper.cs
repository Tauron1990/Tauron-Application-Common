using System.Windows;
using Tauron.Application.Views.Core;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Views
{
    public sealed class ExportNameHelper : ISortableViewExportMetadata
    {
        private readonly DependencyObject _dependencyObject;
        private readonly int _order;

        public ExportNameHelper([NotNull] string name, [NotNull] DependencyObject dependencyObject)
        {
            _dependencyObject = dependencyObject;
            Name = name;
            _order = ViewManager.GetSortOrder(dependencyObject);
        }

        public string Name { get; private set; }

        [NotNull]
        public object GetMeta()
        {
            return this;
        }

        [NotNull]
        public object GetValue()
        {
            return _dependencyObject;
        }

        public int Order
        {
            get { return _order; }
        }
    }
}