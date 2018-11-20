using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using JetBrains.Annotations;

namespace Tauron.Application.Views
{
    [PublicAPI]
    public static class ViewManagerHelper
    {
        public static readonly DependencyProperty SortOrderProperty =
            DependencyProperty.RegisterAttached("SortOrder", typeof(int), typeof(ViewManager), new PropertyMetadata(int.MaxValue));

        public static void SetSortOrder([NotNull] DependencyObject element, int value) => element.SetValue(SortOrderProperty, value);

        public static int GetSortOrder([NotNull] DependencyObject element) => (int)element.GetValue(SortOrderProperty);

        public static readonly DependencyProperty ContentPropertyProperty =
            DependencyProperty.RegisterAttached("ContentProperty", typeof(string), typeof(ViewManager), 
                new FrameworkPropertyMetadata("Content", FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.NotDataBindable));

        [CanBeNull]
        internal static string GetContentProperty([NotNull] DependencyObject element) => (string)element.GetValue(ContentPropertyProperty);

        public static void SetContentProperty([NotNull] DependencyObject dependencyObject, [NotNull] string value) => dependencyObject.SetValue(ContentPropertyProperty, value);




        public static readonly DependencyProperty ImportViewProperty =
            DependencyProperty.RegisterAttached("ImportView", typeof(string), typeof(ViewManager),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.NotDataBindable, ImportViewPropertyChangedCallback));

        private static void ImportViewPropertyChangedCallback([NotNull] DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs
                dependencyPropertyChangedEventArgs)
        {
            if (DesignerProperties.GetIsInDesignMode(dependencyObject))
                return;

            //DEBUG var name = dependencyObject.GetValue(FrameworkElement.NameProperty);
            var viewName = (string)dependencyPropertyChangedEventArgs.NewValue;

            switch (dependencyObject)
            {
                case Panel panel:
                {
                    panel.Children.Clear();
                    foreach (var control in ViewManager.Manager.GetAllViews(viewName)) panel.Children.Add((Control)control);
                    return;
                }
                case ItemsControl itemsCon:
                {
                    itemsCon.Items.Clear();
                    foreach (var control in ViewManager.Manager.GetAllViews(viewName)) itemsCon.Items.Add(control);
                    return;
                }
                case ContentControl contentControl:
                    contentControl.Content = ViewManager.Manager.CreateView(viewName);
                    return;
                case Decorator decorator:
                    decorator.Child = (UIElement)ViewManager.Manager.CreateView(viewName);
                    return;
            }

            var propertyes = TypeDescriptor.GetProperties(dependencyObject);
            var attribute = dependencyObject.GetType().GetCustomAttribute<ContentPropertyAttribute>();
            PropertyDescriptor desc;

            if (attribute != null)
                desc = propertyes.Cast<PropertyDescriptor>().FirstOrDefault(prop => prop.Name == attribute.Name);
            else
            {
                var altName = dependencyObject.GetValue(ContentPropertyProperty) as string;
                desc = !string.IsNullOrEmpty(altName)
                    ? propertyes.Cast<PropertyDescriptor>().FirstOrDefault(prop => prop.Name == altName)
                    : null;
            }

            var viewType = ViewManager.Manager.GetViewType(viewName);

            if (desc == null || !desc.PropertyType.IsAssignableFrom(viewType)) return;

            desc.SetValue(dependencyObject, ViewManager.Manager.CreateView(viewName));
        }

        public static void SetImportView([NotNull] DependencyObject dependencyObject, [NotNull] string value) => dependencyObject.SetValue(ImportViewProperty, value);



        public static readonly DependencyProperty ExportViewProperty =
            DependencyProperty.RegisterAttached("ExportView", typeof(string), typeof(ViewManager),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.NotDataBindable, ExportViewPropertyChanged));

        private static void ExportViewPropertyChanged([NotNull] DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs
                dependencyPropertyChangedEventArgs)
        {
            if (DesignerProperties.GetIsInDesignMode(dependencyObject))
                return;

            var manager = ViewManager.Manager;
            var name = dependencyPropertyChangedEventArgs.NewValue as string;

            if (string.IsNullOrWhiteSpace(name) || !(dependencyObject is Control)) return;

            // ReSharper disable once AssignNullToNotNullAttribute
            manager.Register(new ExportNameHelper(name, dependencyObject));
        }

        public static void SetExportView([NotNull] DependencyObject dependencyObject, [NotNull] string name) => dependencyObject.SetValue(ExportViewProperty, name);
    }
}