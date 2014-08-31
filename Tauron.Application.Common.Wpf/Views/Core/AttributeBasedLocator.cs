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

        public override string GetName(Type model)
        {
            var attr = model.GetCustomAttribute<ExportViewModelAttribute>();
            return attr == null ? null : attr.Name;
        }

        public override DependencyObject Match(ISortableViewExportMetadata name)
        {
            var temp = _views.FirstOrDefault(rs => rs.Metadata == name);

            return temp == null ? null : temp.Resolve(true);
        }

        public override IEnumerable<InstanceResolver<Control, ISortableViewExportMetadata>> GetAllViewsImpl(string name)
        {
            return _views.Where(v => v.Metadata.Name == name);
        }

        public override DependencyObject Match(string name)
        {
            return _views.First(v => v.Metadata.Name == name).Resolve();
        }

        public override IWindow CreateWindowImpl(string name, object[] parameters)
        {
            BuildParameter[] buildParameters = null;
            if (parameters != null)
            {
                buildParameters = new BuildParameter[parameters.Length];
                for (int i = 0; i < parameters.Length; i++)
                {
                    buildParameters[i] = new SimpleBuildPrameter(parameters[i]);
                }
            }

            Window window = _windows.First(win => win.Metadata.Name == name).Resolve(true, buildParameters);

            UiSynchronize.Synchronize.Invoke(() => window.Name = name);

            return new WpfWindow(window);
        }

        public override Type GetViewType(string name)
        {
            return _views.First(vi => vi.Metadata.Name == name).RealType;
        }
    }
}