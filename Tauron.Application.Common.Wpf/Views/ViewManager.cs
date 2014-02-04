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
    public sealed class ViewManager
    {
        #region ExportView
        private class ExportNameHelper : ISortableViewExportMetadata
        {
            private readonly DependencyObject _dependencyObject;
            private readonly int _order;

            public ExportNameHelper([NotNull] string name, [NotNull] DependencyObject dependencyObject)
            {
                _dependencyObject = dependencyObject;
                Name = name;
                _order = GetSortOrder(dependencyObject);
            }

            public string Name { get; private set; }

            [NotNull]
            public object GetMeta()
            {
                return this;
            }

            [NotNull]
            public object GetValue()
            {
                return _dependencyObject;
            }

            public int Order
            {
                get { return _order; }
            }
        }

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

            // ReSharper disable once AssignNullToNotNullAttribute
            var helper = new ExportNameHelper(name, dependencyObject);

            if (dependencyObject is Window)
                manager._windows.Add(new InstanceResolver<Window, INameExportMetadata>(helper.GetValue, helper.GetMeta,
                                                                                            dependencyObject.GetType()));
            else 
                manager._views.Add(new InstanceResolver<Control, ISortableViewExportMetadata>(helper.GetValue, helper.GetMeta,
                                                                                       dependencyObject.GetType()));
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


        [Inject] 
        private List<InstanceResolver<Control, ISortableViewExportMetadata>> _views;

        [Inject] 
        private List<InstanceResolver<Window, INameExportMetadata>> _windows; 

        internal ViewManager()
        {
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

        [NotNull]
        public IWindow CreateWindow([NotNull] string name)
        {
            Contract.Requires<ArgumentNullException>(name != null, "name");

            var win = _windows.First(vi => vi.Metadata.Name == name).Resolve(true);

            UiSynchronize.Synchronize.Invoke(() => win.Name = name);

            return new WpfWindow(win);
        }

        [NotNull]
        public Type GetViewType([NotNull] string name)
        {
            Contract.Requires<ArgumentNullException>(name != null, "name");

            var temp = _views.FirstOrDefault(vi => vi.Metadata.Name == name);
            return temp != null ? temp.RealType : _windows.First(vi => vi.Metadata.Name == name).RealType;
        }

        [CanBeNull]
        public TType CreateView<TType>([NotNull] string name)
            where TType : class
        {
            Contract.Requires<ArgumentNullException>(name != null, "name");

            return CreateView(name) as TType;
        }

        [NotNull]
        public Control CreateView([NotNull] string name)
        {
            Contract.Requires<ArgumentNullException>(name != null, "name");

            return _views.First(view => view.Metadata.Name == name).Resolve(true);
        }

        [NotNull]
        public IEnumerable<Control> GetAllViews([NotNull] string name)
        {
            Contract.Requires<ArgumentNullException>(name != null, "name");

            return _views.Where(res => res.Metadata.Name == name).OrderBy(res => res.Metadata.Order).Select(res => res.Resolve()).ToArray();
        }

        [CanBeNull]
        public IWindow GetWindow([NotNull] string windowName)
        {
            var wind = System.Windows.Application.Current.Windows.Cast<Window>().FirstOrDefault(w => w.Name == windowName);
            return wind == null ? null : new WpfWindow(wind);
        }
    }
}