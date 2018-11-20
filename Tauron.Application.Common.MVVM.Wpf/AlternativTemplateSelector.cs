using System.Windows;
using System.Windows.Controls;

namespace Tauron.Application
{
    public sealed class AlternativTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            // ReSharper disable ConditionIsAlwaysTrueOrFalse
            // ReSharper disable HeuristicUnreachableCode
            if (item == null || container == null) return null;

            // ReSharper restore HeuristicUnreachableCode
            // ReSharper restore ConditionIsAlwaysTrueOrFalse
            var key = item.GetType().Name;
            var ele = container.As<FrameworkElement>();

            var temp = ele?.TryFindResource(key).As<DataTemplate>();
            return temp;
        }
    }
}