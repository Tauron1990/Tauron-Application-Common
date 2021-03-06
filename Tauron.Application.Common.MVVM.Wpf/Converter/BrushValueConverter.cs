﻿using System;
using System.Windows.Data;
using System.Windows.Media;
using JetBrains.Annotations;

namespace Tauron.Application.Converter
{
    public sealed class BrushValueConverter : ValueConverterFactoryBase
    {
        protected override IValueConverter Create() => new Converter();

        private class Converter : ValueConverterBase<string, Brush>
        {
            private static readonly BrushConverter ConverterImpl = new BrushConverter();

            [CanBeNull]
            protected override Brush Convert([NotNull] string value)
            {
                try
                {
                    return ConverterImpl.ConvertFrom(value) as Brush;
                }
                catch (FormatException)
                {
                    return Brushes.Black;
                }
            }
        }
    }
}