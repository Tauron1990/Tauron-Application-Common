using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using JetBrains.Annotations;
using NLog;
using Tauron.Application.Ioc;
using Tauron.Application.Models;
using IContainer = Tauron.Application.Ioc.IContainer;

namespace Tauron.Application.Composition
{
    [PublicAPI]
    public static class CompositionServices
    {
        [NotNull]
        public static IContainer Container => CommonApplication.Current.Container;

        public static BuildParameter[] BuildParameters { get; set; }

        public static readonly DependencyProperty ImportViewModelProperty = 
            DependencyProperty.RegisterAttached("ImportViewModel", typeof(string), typeof(CompositionServices), new PropertyMetadata(default(string), ImportViewModelPropertyChanged));

        public static void SetImportViewModel([NotNull] DependencyObject element, [CanBeNull] string value) => element.SetValue(ImportViewModelProperty, value);

        [NotNull]
        public static string GetImportViewModel([NotNull] DependencyObject element) => (string) element.GetValue(ImportViewModelProperty);

        public static readonly DependencyProperty ImportProperty = 
            DependencyProperty.RegisterAttached("Import", typeof(Type), typeof(CompositionServices), new PropertyMetadata(null, ImportPropertyChanged));

        [CanBeNull]
        public static Type GetImport([NotNull] DependencyObject obj) => (Type) obj.GetValue(ImportProperty);

        public static void SetImport([NotNull] DependencyObject obj, [CanBeNull] Type value) => obj.SetValue(ImportProperty, value);

        private static void ImportPropertyChanged([NotNull] DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(d)) return;

            try
            {
                var adapters = Container.ResolveAll<IViewAggregatorAdapter>(null);
                var adapter = adapters.FirstOrDefault(a => a.CanAdapt(d));

                if (adapter == null)
                {
                    var obj = new FrameworkObject(d);
                    var views = Container.ResolveAll((Type) e.NewValue, null);
                    obj.DataContext = views.FirstOrDefault();
                    return;
                }

                lock (adapter)
                {
                    adapter.Adapt(d);
                    var views = Container.ResolveAll((Type) e.NewValue, null);

                    adapter.AddViews(views);

                    adapter.Release();
                }
            }
            catch (Exception ex)
            {
                LogManager.GetLogger(nameof(CompositionServices), typeof(CompositionServices)).Error(ex);
                throw;
            }
        }

        private static void ImportViewModelPropertyChanged([NotNull] DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(d))
                return;

            var name = e.NewValue as string;

            if (string.IsNullOrWhiteSpace(name)) return;

            // ReSharper disable once ObjectCreationAsStatement
            new FrameworkObject(d, false)
            {
                DataContext = Container.Resolve(typeof(ViewModelBase), name, true, BuildParameters)
            };
        }
    }
}