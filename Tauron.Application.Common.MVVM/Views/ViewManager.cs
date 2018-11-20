using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Tauron.Application.Ioc;
using Tauron.Application.Models;
using Tauron.Application.UIConnector;
using Tauron.Application.Views.Core;

namespace Tauron.Application.Views
{
    [Export(typeof(ViewManager))]
    [PublicAPI]
    public sealed class ViewManager : IViewLocator
    {
        private static IUIConnector _uiConnector;
        private static IUIConnector UiConnector => _uiConnector ?? (_uiConnector = CommonApplication.Current.Container.Resolve<IUIConnector>());

        [CanBeNull]
        private IViewLocator _viewLocator;

        public ViewManager() { }

        public ViewManager([NotNull] IViewLocator locator) => ViewLocator = Argument.NotNull(locator, nameof(locator));

        [NotNull]
        public IViewLocator ViewLocator
        {
            get => _viewLocator ?? UiConnector.ViewLocator;
            set => _viewLocator = value;
        }

        [NotNull]
        public static ViewManager Manager => CommonApplication.Current.Container.Resolve<ViewManager>();


        public IWindow CreateWindow(string name, params object[] parameters) => ViewLocator.CreateWindow(name, parameters);

        public Type GetViewType(string name) => ViewLocator.GetViewType(name);

        public object CreateView(string name) => ViewLocator.CreateView(name);

        public IEnumerable<object> GetAllViews(string name) => ViewLocator.GetAllViews(name);

        public void Register(ExportNameHelper export) => ViewLocator.Register(export);

        public object CreateViewForModel(object model) => ViewLocator.CreateViewForModel(model);

        public object CreateViewForModel(Type model) => ViewLocator.CreateViewForModel(model);

        public string GetName(ViewModelBase model) => ViewLocator.GetName(model);

        [CanBeNull]
        public TType CreateView<TType>([NotNull] string name)
            where TType : class => CreateView(Argument.NotNull(name, nameof(name))) as TType;

        [CanBeNull]
        public IWindow GetWindow([NotNull] string windowName) => UiSynchronize.Synchronize.Invoke(() => UiConnector.ViewManagerConnector.GetWindow(windowName));

        public static int GetSortOrder(object @object) => UiConnector.ViewManagerConnector.GetSortOrder(@object);
    }
}