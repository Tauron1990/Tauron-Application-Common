using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Tauron.Application.Models;

namespace Tauron.Application.Views.Core
{
    public abstract class CommonLocatorBase : IViewLocator
    {
        private Dictionary<string, ExportNameHelper> _views = new Dictionary<string, ExportNameHelper>();

        public void Register(ExportNameHelper export) => _views[export.Name] = export;

        public object CreateViewForModel(object model)
        {
            var temp = CreateViewForModel(model.GetType());
            SetDataContext(temp, model);
            //new FrameworkObject(temp, false).DataContext = model;

            return temp;
        }

        public object CreateViewForModel(Type model)
        {
            var name = GetName(model);
            if (name == null) return null;

            var temp = NamingHelper.CreatePossibilyNames(name)
                .Select(Match)
                .FirstOrDefault(view => view != null);

            if (temp != null) return temp;

            temp = _views.First(v => v.Key == name).Value.GetValue();
            if (IsWindow(temp)) temp = null;

            return temp;
        }

        public object CreateView(string name) => Match(name);

        public IWindow CreateWindow(string name, object[] parameters)
        {
            if (!_views.TryGetValue(name, out var export)) return CreateWindowImpl(name, parameters);

            var temp = export.GetValue();

            return IsWindow(temp) ? CreateWindow(temp) : CreateWindowImpl(name, parameters);
        }

        public abstract Type GetViewType(string name);

        public IEnumerable<object> GetAllViews(string name) => GetAllViewsImpl(name).OrderBy(meta => meta.Metadata.Order).Select(i => i.Resolve());

        public string GetName(ViewModelBase model) => GetName(model.GetType());


        [NotNull]
        public abstract object Match([NotNull] string name);

        [CanBeNull]
        public abstract string GetName([NotNull] Type model);

        [CanBeNull]
        public abstract object Match([NotNull] ISortableViewExportMetadata name);

        [NotNull]
        public abstract IEnumerable<(Func<object> Resolve, ISortableViewExportMetadata Metadata)> GetAllViewsImpl([NotNull] string name);

        [NotNull]
        public abstract IWindow CreateWindowImpl([NotNull] string name, [CanBeNull] object[] parameters);

        protected abstract IWindow CreateWindow(object view);

        protected abstract bool IsWindow(object view);

        protected abstract void SetDataContext(object view, object model);
    }
}