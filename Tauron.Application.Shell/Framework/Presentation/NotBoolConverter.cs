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

#region Usings

using System;
using System.Globalization;
using System.Windows.Data;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Shell.Framework.Presentation
{
    [PublicAPI]
	public class NotBoolConverter : IValueConverter
	{
	    [NotNull]
	    public object Convert([NotNull] object value, [NotNull] Type targetType, [NotNull] object parameter, [NotNull] CultureInfo culture)
	    {
	        return !(value is bool) || !(bool) value;
	    }

	    [NotNull]
	    public object ConvertBack([NotNull] object value, [NotNull] Type targetType, [NotNull] object parameter, [NotNull] CultureInfo culture)
	    {
	        return !(value is bool) || !(bool) value;
	    }
	}

    [PublicAPI]
    public class ContextMenuBuilder : IMultiValueConverter
    {
        [NotNull]
        public object Convert([NotNull] object[] values, [NotNull] Type targetType, [NotNull] object parameter,
                              [NotNull] CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is string) return MenuService.CreateContextMenu(values[1], (string) values[0]);
            throw new NotSupportedException();
        }

        [NotNull]
        public object[] ConvertBack([NotNull] object value, [NotNull] Type[] targetTypes, [NotNull] object parameter,
                                    [NotNull] CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
