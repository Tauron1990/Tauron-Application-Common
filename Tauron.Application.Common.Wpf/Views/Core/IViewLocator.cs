using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Windows;
using Tauron.Application.Models;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Views.Core
{
    [PublicAPI, ContractClass(typeof(ViewLocatorContracts))]
    public interface IViewLocator
    {
        void Register([NotNull] ExportNameHelper export);

        [CanBeNull]
        DependencyObject CreateViewForModel([NotNull] object model);

        [CanBeNull]
        DependencyObject CreateViewForModel([NotNull] Type model);

        [NotNull]
        DependencyObject CreateView([NotNull] string name);

        [NotNull]
        IWindow CreateWindow([NotNull] string name, [CanBeNull] object[] parameters);

        [NotNull]
        Type GetViewType([NotNull] string name);

        [NotNull]
        IEnumerable<DependencyObject> GetAllViews([NotNull] string name);

        [CanBeNull]
        string GetName([NotNull] ViewModelBase model);
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
            Contract.Ensures(Contract.Result<DependencyObject>() != null);

            return null;
        }

        public DependencyObject CreateViewForModel(Type model)
        {
            Contract.Requires<ArgumentNullException>(model != null, "model");
            Contract.Ensures(Contract.Result<DependencyObject>() != null);

            return null;
        }

        public DependencyObject CreateView(string name)
        {
            Contract.Requires<ArgumentNullException>(name != null, "name");
            Contract.Ensures(Contract.Result<DependencyObject>() != null);
            return null;
        }

        public IWindow CreateWindow(string name, object[] parameters)
        {
            Contract.Requires<ArgumentNullException>(name != null, "name");
            Contract.Ensures(Contract.Result<IWindow>() != null);
            return null;
        }

        public Type GetViewType(string name)
        {
            Contract.Requires<ArgumentNullException>(name != null, "name");
            Contract.Ensures(Contract.Result<Type>() != null);
            return null;
        }

        public IEnumerable<DependencyObject> GetAllViews(string name)
        {
            Contract.Requires<ArgumentNullException>(name != null, "name");
            Contract.Ensures(Contract.Result<IEnumerable<DependencyObject>>() != null);
            return null;

        }

        public string GetName(ViewModelBase model)
        {
            Contract.Requires<ArgumentNullException>(model != null, "model");
            return null;
        }
    }
}