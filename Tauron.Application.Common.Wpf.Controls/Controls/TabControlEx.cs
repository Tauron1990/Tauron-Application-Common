using System.Windows;
using System.Windows.Controls;
using Tauron.Application.Models;
using Tauron.Application.Views;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Controls
{
    public class TabControlEx : TabControl
    {
        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register(
            "HeaderTemplate", typeof (DataTemplate), typeof (TabControlEx), new PropertyMetadata(default(DataTemplate)));

        [CanBeNull]
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate) GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TabItem { HeaderTemplate = HeaderTemplate };
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            var tabItem = (TabItem) element;
            var model = item as ViewModelBase;

            if(model == null) return;

            tabItem.Content = ViewManager.Manager.CreateViewForModel(model);
        }
    }
}
