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

        protected override DependencyObject Match(string name)
        {
            var temp = _views.FirstOrDefault(rs => rs.Metadata.Name == name);
            
            return temp == null ? null : temp.Resolve(true);
        }

        public override DependencyObject CreateView(string name)
        {
            return _views.First(view => view.Metadata.Name == name).Resolve(true);
        }

        public override IWindow CreateWindow(string name)
        {
            var win = _windows.First(vi => vi.Metadata.Name == name).Resolve(true);

            UiSynchronize.Synchronize.Invoke(() => win.Name = name);

            return new WpfWindow(win);
        }

        public override Type GetViewType(string name)
        {
            var temp = _views.FirstOrDefault(vi => vi.Metadata.Name == name);
            return temp != null ? temp.RealType : _windows.First(vi => vi.Metadata.Name == name).RealType;
        }

        public override IEnumerable<Control> GetAllViews(string name)
        {
            return _views.Where(res => res.Metadata.Name == name).OrderBy(res => res.Metadata.Order).Select(res => res.Resolve()).ToArray();
        }
    }
}