using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Tauron.Application.Composition;
using Tauron.Application.Ioc;
using Tauron.Application.Views.Core;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Views
{
    [Export(typeof (ViewManager))]
    [PublicAPI]
    public sealed class ViewManager : IViewLocator
    {
        #region ExportView
        
        public static readonly DependencyProperty ExportViewProperty =
            DependencyProperty.RegisterAttached("ExportView", typeof (string), typeof (ViewManager),
                                                new FrameworkPropertyMetadata(string.Empty,
                                                                              FrameworkPropertyMetadataOptions
                                                                                  .NotDataBindable,
                                                                              ExportViewPropertyChanged));

        private static void ExportViewPropertyChanged([NotNull] DependencyObject dependencyObject,
                                                      DependencyPropertyChangedEventArgs
                                                          dependencyPropertyChangedEventArgs)
        {
            if(DesignerProperties.GetIsInDesignMode(dependencyObject))
                return;

            ViewManager manager = Manager;
            var name = dependencyPropertyChangedEventArgs.NewValue as string;

            if (string.IsNullOrWhiteSpace(name) || (dependencyObject as Control) == null) return;

            manager.Register( new ExportNameHelper(name, dependencyObject));
        }

        public static void SetExportView([NotNull] DependencyObject dependencyObject, [NotNull] string name)
        {
            dependencyObject.SetValue(ExportViewProperty, name);
        }

        #endregion

        #region Import View

        public static readonly DependencyProperty ImportViewProperty =
            DependencyProperty.RegisterAttached("ImportView", typeof (string), typeof (ViewManager),
                                                new FrameworkPropertyMetadata(string.Empty,
                                                                              FrameworkPropertyMetadataOptions
                                                                                  .NotDataBindable,
                                                                              ImportViewPropertyChangedCallback));

        private static void ImportViewPropertyChangedCallback([NotNull] DependencyObject dependencyObject,
                                                    DependencyPropertyChangedEventArgs
                                                        dependencyPropertyChangedEventArgs)
        {
            if (DesignerProperties.GetIsInDesignMode(dependencyObject))
                return;

            var name = dependencyObject.GetValue(FrameworkElement.NameProperty);
            var viewName = (string) dependencyPropertyChangedEventArgs.NewValue;

            var panel = dependencyObject as Panel;
            if (panel != null)
            {
                panel.Children.Clear();
                foreach (Control control in Manager.GetAllViews(viewName)) panel.Children.Add(control);
                return;
            }

            var itemsCon = dependencyObject as ItemsControl;
            if (itemsCon != null)
            {
                itemsCon.Items.Clear();
                foreach (var control in Manager.GetAllViews(viewName)) itemsCon.Items.Add(control);
                return;
            }

            var contentControl = dependencyObject as ContentControl;
            if (contentControl != null)
            {
                contentControl.Content = Manager.CreateView(viewName);
                return;
            }

            PropertyDescriptorCollection propertyes = TypeDescriptor.GetProperties(dependencyObject);
            var attribute = dependencyObject.GetType().GetCustomAttribute<ContentPropertyAttribute>();
            PropertyDescriptor desc;

            if (attribute != null) desc = propertyes.Cast<PropertyDescriptor>().FirstOrDefault(prop => prop.Name == attribute.Name);
            else
            {
                var altName = dependencyObject.GetValue(ContentPropertyProperty) as string;
                desc = !string.IsNullOrEmpty(altName)
                           ? propertyes.Cast<PropertyDescriptor>().FirstOrDefault(prop => prop.Name == altName)
                           : null;
            }

            Type viewType = Manager.GetViewType(viewName);

            if (desc == null || !desc.PropertyType.IsAssignableFrom(viewType)) return;

            desc.SetValue(dependencyObject, Manager.CreateView(viewName));
        }

        public static void SetImportView([NotNull] DependencyObject dependencyObject, [NotNull] string value)
        {
            dependencyObject.SetValue(ImportViewProperty, value);
        }

        #endregion

        #region Content Property

        public static readonly DependencyProperty ContentPropertyProperty =
            DependencyProperty.RegisterAttached("ContentProperty", typeof (string), typeof (ViewManager),
                                                new FrameworkPropertyMetadata("Content",
                                                                              FrameworkPropertyMetadataOptions.Inherits |
                                                                              FrameworkPropertyMetadataOptions
                                                                                  .NotDataBindable));

        [CanBeNull]
        internal static string GetContentProperty([NotNull] DependencyObject element)
        {
            return (string) element.GetValue(ContentPropertyProperty);
        }

        public static void SetContentProperty([NotNull] DependencyObject dependencyObject, [NotNull] string value)
        {
            dependencyObject.SetValue(ContentPropertyProperty, value);
        }

        #endregion

        #region SortOrder

        public static readonly DependencyProperty SortOrderProperty =
            DependencyProperty.RegisterAttached("SortOrder", typeof (int), typeof (ViewManager),
                                                new PropertyMetadata(int.MaxValue));

        public static void SetSortOrder([NotNull] DependencyObject element, int value)
        {
            element.SetValue(SortOrderProperty, value);
        }

        public static int GetSortOrder([NotNull] DependencyObject element)
        {
            return (int) element.GetValue(SortOrderProperty);
        }

        #endregion

        [NotNull]
        public IViewLocator ViewLocator { get; private set; }

        public ViewManager()
        {
            ViewLocator = Factory.Object<AttributeBasedLocator>();
        }

        public ViewManager([NotNull] IViewLocator locator)
        {
            Contract.Requires<ArgumentNullException>(locator != null, "locator");
            ViewLocator = locator;
        }

        [NotNull]
        public static ViewManager Manager
        {
            get
            {
                Contract.Ensures(Contract.Result<ViewManager>() != null);

                return CompositionServices.Container.Resolve<ViewManager>();
            }
        }

        public IWindow CreateWindow(string name)
        {
            return ViewLocator.CreateWindow(name);
        }

        public Type GetViewType(string name)
        {
            return ViewLocator.GetViewType(name);
        }

        [CanBeNull]
        public TType CreateView<TType>([NotNull] string name)
            where TType : class
        {
            Contract.Requires<ArgumentNullException>(name != null, "name");

            return CreateView(name) as TType;
        }

        public DependencyObject CreateView(string name)
        {
            return ViewLocator.CreateView(name);
        }

        public IEnumerable<DependencyObject> GetAllViews(string name)
        {
            return ViewLocator.GetAllViews(name);
        }

        [CanBeNull]
        public IWindow GetWindow([NotNull] string windowName)
        {
            var wind = System.Windows.Application.Current.Windows.Cast<Window>().FirstOrDefault(w => w.Name == windowName);
            return wind == null ? null : new WpfWindow(wind);
        }

        public void Register(ExportNameHelper export)
        {
            ViewLocator.Register(export);
        }

        public DependencyObject CreateViewForModel(object model)
        {
            return ViewLocator.CreateViewForModel(model);
        }

        public DependencyObject CreateViewForModel(Type model)
        {
            return ViewLocator.CreateViewForModel(model);
        }

    }
}