using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using JetBrains.Annotations;
using Tauron.Application.Composition;
using Tauron.Application.Ioc;
using Tauron.Application.Models;
using Tauron.Application.Views.Core;

namespace Tauron.Application.UIConnector
{
    [PublicAPI]
    public sealed class AttributeBasedLocator : CommonLocatorBase
    {
        [Inject]
        private List<InstanceResolver<Control, ISortableViewExportMetadata>> _views;

        [Inject]
        private List<InstanceResolver<Window, INameExportMetadata>> _windows;

        public override string GetName(Type model)
        {
            var attr = model.GetCustomAttribute<ExportViewModelAttribute>();
            return attr?.Name;
        }

        public override object Match(ISortableViewExportMetadata name)
        {
            var temp = _views.FirstOrDefault(rs => rs.Metadata == name);

            return temp?.Resolve(true);
        }

        public override IEnumerable<(Func<object> Resolve, ISortableViewExportMetadata Metadata)> GetAllViewsImpl(string name) 
            => _views.Where(v => v.Metadata.Name == name)
                .Select(instanceResolver => ((Func<object> Resolve, ISortableViewExportMetadata Metadata)) (() => instanceResolver.Resolve(), instanceResolver.Metadata));

        public override object Match(string name) => _views.First(v => v.Metadata.Name == name).Resolve();

        public override IWindow CreateWindowImpl(string name, object[] parameters)
        {
            try
            {
                BuildParameter[] buildParameters = null;
                if (parameters != null)
                {
                    buildParameters = new BuildParameter[parameters.Length];
                    for (var i = 0; i < parameters.Length; i++)
                    {
                        var oParm = parameters[i];
                        if (oParm is BuildParameter buildParameter)
                            buildParameters[i] = buildParameter;
                        else
                            buildParameters[i] = new SimpleBuildPrameter(parameters[i]);
                    }
                }

                CompositionServices.BuildParameters = buildParameters;
                var window = _windows.First(win => win.Metadata.Name == name).ResolveRaw(true, buildParameters);

                return CastToWindow(window, name);
            }
            finally
            {
                CompositionServices.BuildParameters = null;
            }
        }

        protected override IWindow CreateWindow(object view) => new WpfWindow((Window)view);

        protected override bool IsWindow(object view) => view is Window;

        protected override void SetDataContext(object view, object model) => new FrameworkObject(view).DataContext = model;

        public override Type GetViewType(string name) => _views.First(vi => vi.Metadata.Name == name).RealType;

        private IWindow CastToWindow(object objWindow, string name)
        {
            var window = (Window) objWindow;

            UiSynchronize.Synchronize.Invoke(() => window.Name = name);

            return new WpfWindow(window);
        }
    }
}