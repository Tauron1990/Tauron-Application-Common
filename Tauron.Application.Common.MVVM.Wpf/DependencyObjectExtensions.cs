using System.Windows;
using JetBrains.Annotations;

namespace Tauron.Application
{
    [PublicAPI]
    public static class DependencyObjectExtensions
    {
        [CanBeNull]
        public static object FindResource([CanBeNull] this DependencyObject obj, [NotNull] object key)
        {
            Argument.NotNull(key, nameof(key));

            var temp1 = obj as FrameworkElement;
            var temp2 = obj as FrameworkContentElement;
            object result = null;
            if (temp1 != null) result = temp1.TryFindResource(key);

            if (result == null && temp2 != null) result = temp2.TryFindResource(key);

            return result;
        }


        [CanBeNull]
        public static TType FindResource<TType>([NotNull] this DependencyObject obj, [NotNull] object key) where TType : class => FindResource(obj, key) as TType;
    }
}