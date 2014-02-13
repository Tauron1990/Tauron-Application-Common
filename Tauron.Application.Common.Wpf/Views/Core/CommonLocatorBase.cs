using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Windows;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Views.Core
{
    [ContractClass(typeof(CommonLocatorBaseContracts))]
    public abstract class CommonLocatorBase : IViewLocator
    {
        private Dictionary<string, ExportNameHelper> _views = new Dictionary<string, ExportNameHelper>(); 

        public void Register(ExportNameHelper export)
        {
            _views[export.Name] = export;
        }

        public DependencyObject CreateViewForModel(object model)
        {
            return CreateViewForModel(model.GetType());
        }

        public DependencyObject CreateViewForModel(Type model)
        {
            string name = GetName(model);
            if (name == null) return null;

            var temp = NamingHelper.CreatePossibilyNames(name)
                .Select(Match)
                .FirstOrDefault(view => view != null);

            if (temp != null) return temp;

            temp = _views.First(v => v.Key == name).Value.GetValue() as DependencyObject;
            if (temp is Window) temp = null;

            return temp;
        }


        [NotNull]
        protected abstract DependencyObject Match([NotNull] string name);

        [CanBeNull]
        protected abstract string GetName([NotNull] Type model);

        [CanBeNull]
        protected abstract DependencyObject Match([NotNull] ISortableViewExportMetadata name);

        [NotNull]
        protected abstract IEnumerable<ISortableViewExportMetadata> GetAllViewsImpl([NotNull] string name);

        public DependencyObject CreateView(string name)
        {
            return Match(name);
        }

        public IWindow CreateWindow(string name, object[] parameters)
        {
            ExportNameHelper export;
            if (_views.TryGetValue(name, out export))
            {
                var win = export.GetValue() as Window;
                if(win != null)
                    return new WpfWindow(win);
            }

            return CreateWindowImpl(name, parameters);
        }

        [NotNull]
        public abstract IWindow CreateWindowImpl([NotNull] string name, [CanBeNull]object[] parameters);
        public abstract Type GetViewType(string name);

        public IEnumerable<DependencyObject> GetAllViews(string name)
        {
            return GetAllViewsImpl(name).OrderBy(meta => meta.Order).Select(Match);
        }
    }

    [ContractClassFor(typeof(CommonLocatorBase))]
    abstract class CommonLocatorBaseContracts : CommonLocatorBase
    {
        protected override string GetName(Type model)
        {
            Contract.Requires<ArgumentNullException>(model != null, "model");
            return null;
        }

        protected override DependencyObject Match(ISortableViewExportMetadata name)
        {
            Contract.Requires<ArgumentNullException>(name != null, "name");
            return null;
        }

        protected override IEnumerable<ISortableViewExportMetadata> GetAllViewsImpl(string name)
        {
            Contract.Requires<ArgumentNullException>(name != null, "name");
            return null;
        }

        public override IWindow CreateWindowImpl(string name, object[] parameters)
        {
            Contract.Requires<ArgumentNullException>(name != null, "name");
            return null;
        }

        public override Type GetViewType(string name)
        {
            Contract.Requires<ArgumentNullException>(name != null, "name");
            return null;
        }
    }
}