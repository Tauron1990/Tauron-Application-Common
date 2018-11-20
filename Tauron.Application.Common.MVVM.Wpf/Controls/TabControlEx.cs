using System.Windows;
using System.Windows.Controls;
using JetBrains.Annotations;
using Tauron.Application.Models;
using Tauron.Application.Views;

namespace Tauron.Application.Controls
{
    [PublicAPI]
    public class TabControlEx : TabControl
    {
        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register(
            "HeaderTemplate", typeof(DataTemplate), typeof(TabControlEx),
            new PropertyMetadata(default(DataTemplate)));

        [CanBeNull]
        public DataTemplate HeaderTemplate
        {
            get => (DataTemplate) GetValue(HeaderTemplateProperty);
            set => SetValue(HeaderTemplateProperty, value);
        }

        protected override DependencyObject GetContainerForItemOverride() => new TabItem {HeaderTemplate = HeaderTemplate};

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            var tabItem = (TabItem) element;

            if (!(item is ViewModelBase model)) return;

            tabItem.Content = ViewManager.Manager.CreateViewForModel(model);
        }
    }
}