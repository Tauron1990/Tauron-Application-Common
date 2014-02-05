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
        private Dictionary<string, ExportNameHelper> _helpers; 

        public void Register(ExportNameHelper export)
        {
        
        }

        public DependencyObject CreateViewForModel(object model)
        {
            return CreateViewForModel(model.GetType());
        }

        public DependencyObject CreateViewForModel(Type model)
        {
            string name = GetName(model);
            if (name == null) return null;

            return NamingHelper.CreatePossibilyNames(name)
                .Select(Match)
                .FirstOrDefault(view => view != null);
        }

        [NotNull]
        protected abstract string GetName([NotNull] Type model);

        [NotNull]
        protected abstract DependencyObject Match([NotNull] string name);

        [NotNull]
        protected abstract IEnumerable<ISortableViewExportMetadata> GetAllViewsImpl([NotNull] string name);

        public DependencyObject CreateView(string name)
        {
            return Match(name);
        }

        public abstract IWindow CreateWindow(string name);
        public abstract Type GetViewType(string name);

        public IEnumerable<DependencyObject> GetAllViews(string name)
        {
            return GetAllViewsImpl(name).OrderBy(meta => meta.Order).Select(meta => meta.Name).Select(Match);
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

        protected override DependencyObject Match(string name)
        {
            Contract.Requires<ArgumentNullException>(name != null, "name");
            return null;
        }

        protected override IEnumerable<string> GetAllViewsImpl(string name)
        {
            Contract.Requires<ArgumentNullException>(name != null, "name");
            return null;
        }

        public override IWindow CreateWindow(string name)
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