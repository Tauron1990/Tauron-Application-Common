using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Windows;
using System.Windows.Controls;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Views.Core
{
    [PublicAPI, ContractClass(typeof(ViewLocatorContracts))]
    public interface IViewLocator
    {
        void Register([NotNull] ExportNameHelper export);

        [NotNull]
        DependencyObject CreateViewForModel([NotNull] object model);

        [NotNull]
        DependencyObject CreateViewForModel([NotNull] Type model);

        [NotNull]
        DependencyObject CreateView([NotNull] string name);

        [NotNull]
        IWindow CreateWindow([NotNull] string name);

        [NotNull]
        Type GetViewType([NotNull] string name);

        [NotNull]
        IEnumerable<DependencyObject> GetAllViews([NotNull] string name);

    }

    [ContractClassFor(typeof(IViewLocator))]
    abstract class ViewLocatorContracts : IViewLocator
    {
        public void Register(ExportNameHelper export)
        {
            Contract.Requires<ArgumentNullException>(export != null, "export");   
        }

        public DependencyObject CreateViewForModel(object model)
        {
            Contract.Requires<ArgumentNullException>(model != null, "model");
            Contract.EnsuresOnThrow<KeyNotFoundException>(Contract.Result<DependencyObject>() != null);

            return null;
        }

        public DependencyObject CreateViewForModel(Type model)
        {
            Contract.Requires<ArgumentNullException>(model != null, "model");
            Contract.EnsuresOnThrow<KeyNotFoundException>(Contract.Result<DependencyObject>() != null);

            return null;
        }

        public DependencyObject CreateView(string name)
        {
            Contract.Requires<ArgumentNullException>(name != null, "name");
            Contract.EnsuresOnThrow<KeyNotFoundException>(Contract.Result<DependencyObject>() != null);
            return null;
        }

        public IWindow CreateWindow(string name)
        {
            Contract.Requires<ArgumentNullException>(name != null, "name");
            Contract.EnsuresOnThrow<KeyNotFoundException>(Contract.Result<IWindow>() != null);
            return null;
        }

        public Type GetViewType(string name)
        {
            Contract.Requires<ArgumentNullException>(name != null, "name");
            Contract.EnsuresOnThrow<KeyNotFoundException>(Contract.Result<Type>() != null);
            return null;
        }

        public IEnumerable<DependencyObject> GetAllViews(string name)
        {
            Contract.Requires<ArgumentNullException>(name != null, "name");
            Contract.EnsuresOnThrow<KeyNotFoundException>(Contract.Result<IEnumerable<Control>>() != null);
            return null;

        }
    }
}