// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System.Windows;
using System.Windows.Media;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Shell.Framework.Presentation
{
    [PublicAPI]
    public static class WpfTreeNavigation
    {
        /// <summary>
        /// Returns the first occurence of object of type <paramref name="T" /> in the visual tree of <paramref name="System.windows.DependencyObject" />.
        /// <param name="root">Start node.</param>
        /// </summary>
        [CanBeNull]
        public static T TryFindChild<T>([NotNull] DependencyObject root) where T : DependencyObject
        {
            if (root is T) return (T) root;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
            {
                var foundChild = TryFindChild<T>(VisualTreeHelper.GetChild(root, i));
                if (foundChild != null) return foundChild;
            }
            return null;
        }

        /// <summary>
        /// Returns the first occurence of object of type <paramref name="T" /> in the visual tree of <paramref name="dependencyObject" />.
        /// </summary>
        /// <param name="child">Start node</param>
        /// <returns></returns>
        [CanBeNull]
        public static T TryFindParent<T>([NotNull] DependencyObject child, bool includeItSelf = true)
            where T : DependencyObject
        {
            while (true)
            {
                if (includeItSelf && child is T) return child as T;

                DependencyObject parentObject = GetParentObject(child);
                if (parentObject == null) return null;

                var parent = parentObject as T;
                if (parent != null) return parent;
                child = parentObject;
                includeItSelf = true;
            }
        }

        [CanBeNull]
        private static DependencyObject GetParentObject([CanBeNull] DependencyObject child)
        {
            if (child == null) return null;

            DependencyObject parent;
            var contentElement = child as ContentElement;
            if (contentElement != null)
            {
                parent = ContentOperations.GetParent(contentElement);
                if (parent != null) return parent;

                var fce = contentElement as FrameworkContentElement;
                return fce != null ? fce.Parent : null;
            }

            var frameworkElement = child as FrameworkElement;
            if (frameworkElement == null) return VisualTreeHelper.GetParent(child);
            parent = frameworkElement.Parent;
            return parent ?? VisualTreeHelper.GetParent(child);
        }
    }
}
