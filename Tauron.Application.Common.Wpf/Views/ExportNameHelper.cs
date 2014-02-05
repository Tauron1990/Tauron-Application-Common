using System;
using System.Diagnostics.Contracts;
using System.Windows;
using Tauron.Application.Views.Core;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Views
{
    public sealed class ExportNameHelper : ISortableViewExportMetadata
    {
        private readonly DependencyObject _dependencyObject;
        private readonly int _order;
        private string _name;

        public ExportNameHelper([NotNull] string name, [NotNull] DependencyObject dependencyObject)
        {
            Contract.Requires<ArgumentNullException>(name != null, "name");
            Contract.Requires<ArgumentNullException>(dependencyObject != null, "dependencyObject");

            _dependencyObject = dependencyObject;
            Name = name;
            _order = ViewManager.GetSortOrder(dependencyObject);
        }

        public string Name
        {
            get
            {
                Contract.Ensures(Contract.Result<string>() != null);
                return _name;
            }
            private set { _name = value; }
        }

        [NotNull]
        public object GetMeta()
        {
            Contract.Ensures(Contract.Result<object>() != null);

            return this;
        }

        [NotNull]
        public object GetValue()
        {
            Contract.Ensures(Contract.Result<object>() != null);

            return _dependencyObject;
        }

        public int Order
        {
            get { return _order; }
        }
    }
}