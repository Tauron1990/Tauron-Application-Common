using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Tauron.Application.Ioc;
using Tauron.Application.Models;

namespace Tauron.Application.Views.Core
{
    [Export(typeof(IViewLocator))]
    public class AttributeBasedLocator : IViewLocator
    {
        [Inject]
        private List<InstanceResolver<Control, ISortableViewExportMetadata>> _views;

        [Inject]
        private List<InstanceResolver<Window, INameExportMetadata>> _windows; 

        public DependencyObject CreateViewForModel(object model)
        {
            return CreateViewForModel(model.GetType());
        }

        public DependencyObject CreateViewForModel(Type model)
        {
            var attr = model.GetCustomAttribute<ExportModelAttribute>();
            if (attr == null) return null;
        }

        public DependencyObject CreateView(string name)
        {
        ???
        }

        public Window CreateWindow(string name)
        {
        ???
        }
    }
}