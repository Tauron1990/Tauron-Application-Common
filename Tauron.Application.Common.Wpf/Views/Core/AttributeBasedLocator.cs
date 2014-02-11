using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Tauron.Application.Ioc;
using Tauron.Application.Models;

namespace Tauron.Application.Views.Core
{
    public class AttributeBasedLocator : CommonLocatorBase
    {
        [Inject]
        private List<InstanceResolver<Control, ISortableViewExportMetadata>> _views;

        [Inject]
        private List<InstanceResolver<Window, INameExportMetadata>> _windows;

        protected override string GetName(Type model)
        {
            var attr = model.GetCustomAttribute<ExportModelAttribute>();
            return attr == null ? null : attr.Name;
        }

        protected override DependencyObject Match(ISortableViewExportMetadata name)
        {
            var temp = _views.FirstOrDefault(rs => rs.Metadata == name);

            return temp == null ? null : temp.Resolve(true);
        }

        protected override IEnumerable<ISortableViewExportMetadata> GetAllViewsImpl(string name)
        {
            return _views.Where(v => v.Metadata.Name == name).Select(v => v.Metadata);
        }

        protected override DependencyObject Match(string name)
        {
            return _views.First(v => v.Metadata.Name == name).Resolve();
        }

        public override IWindow CreateWindowImpl(string name)
        {
            Window window = _windows.First(win => win.Metadata.Name == name).Resolve(true);

            UiSynchronize.Synchronize.Invoke(() => window.Name = name);

            return new WpfWindow(window);
        }

        public override Type GetViewType(string name)
        {
            return _views.First(vi => vi.Metadata.Name == name).RealType;
        }
    }
}