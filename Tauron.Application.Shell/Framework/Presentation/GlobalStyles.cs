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
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Shell.Framework.Presentation
{
    /// <summary>
    /// Contains global WPF styles are used.
    /// </summary>
    [PublicAPI]
    public static class GlobalStyles
    {
        [CanBeNull]
        private static Style FindResource([NotNull] ResourceKey key)
        {
            // don't crash if controls using GlobalStyles are instanciated in unit test mode
            if (System.Windows.Application.Current == null) return null;
            return (Style) System.Windows.Application.Current.FindResource(key);
        }

        [CanBeNull]
        public static Style WindowStyle
        {
            get { return FindResource(WindowStyleKeyPrivate); }
        }

        [NotNull]
        private static readonly ResourceKey WindowStyleKeyPrivate = new ComponentResourceKey(typeof (GlobalStyles),
                                                                                      "WindowStyle");

        [NotNull]
        public static ResourceKey WindowStyleKey
        {
            get { return WindowStyleKeyPrivate; }
        }

        [CanBeNull]
        public static Style DialogWindowStyle
        {
            get { return FindResource(dialogWindowStyleKey); }
        }

        private static readonly ResourceKey dialogWindowStyleKey = new ComponentResourceKey(typeof (GlobalStyles),
                                                                                            "DialogWindowStyle");

        [NotNull]
        public static ResourceKey DialogWindowStyleKey
        {
            get { return dialogWindowStyleKey; }
        }

        [CanBeNull]
        public static Style ButtonStyle
        {
            get { return FindResource(buttonStyleKey); }
        }

        private static readonly ResourceKey buttonStyleKey = new ComponentResourceKey(typeof (GlobalStyles),
                                                                                      "ButtonStyle");

        [NotNull]
        public static ResourceKey ButtonStyleKey
        {
            get { return buttonStyleKey; }
        }

        [CanBeNull]
        public static Style WordWrapCheckBoxStyle
        {
            get { return FindResource(wordWrapCheckBoxStyleKey); }
        }

        private static readonly ResourceKey wordWrapCheckBoxStyleKey = new ComponentResourceKey(typeof (GlobalStyles),
                                                                                                "WordWrapCheckBoxStyle");

        [NotNull]
        public static ResourceKey WordWrapCheckBoxStyleKey
        {
            get { return wordWrapCheckBoxStyleKey; }
        }

        private static readonly ResourceKey flowDirectionKey = new ComponentResourceKey(typeof (GlobalStyles),
                                                                                        "FlowDirectionKey");

        [NotNull]
        public static ResourceKey FlowDirectionKey
        {
            get { return flowDirectionKey; }
        }

        private static readonly ResourceKey listViewItemFocusHighlightStyleKey =
            new ComponentResourceKey(typeof (GlobalStyles), "ListViewItemFocusHighlightStyle");

        [NotNull]
        public static ResourceKey ListViewItemFocusHighlightStyleKey
        {
            get { return listViewItemFocusHighlightStyleKey; }
        }

        [CanBeNull]
        public static Style ListViewItemFocusHighlightStyle
        {
            get { return FindResource(listViewItemFocusHighlightStyleKey); }
        }

        private static readonly ResourceKey listBoxItemFocusHighlightStyleKey =
            new ComponentResourceKey(typeof (GlobalStyles), "ListBoxItemFocusHighlightStyle");

        [NotNull]
        public static ResourceKey ListBoxItemFocusHighlightStyleKey
        {
            get { return listBoxItemFocusHighlightStyleKey; }
        }

        [CanBeNull]
        public static Style ListBoxItemFocusHighlightStyle
        {
            get { return FindResource(listBoxItemFocusHighlightStyleKey); }
        }
    }
}
