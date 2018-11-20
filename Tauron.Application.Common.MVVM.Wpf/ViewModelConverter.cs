using System;
using System.Globalization;
using System.Windows.Data;
using Tauron.Application.Converter;

namespace Tauron.Application
{
    public sealed class ViewModelConverterExtension : ValueConverterFactoryBase
    {
        public ViewModelConverterExtension() => EnableCaching = true;

        public bool EnableCaching { get; set; }

        protected override IValueConverter Create() => new WpfViewModelConverter(EnableCaching);
    }

    public class WpfViewModelConverter : ViewModelConverter, IValueConverter
    {
        public WpfViewModelConverter(bool enableCaching) : base(enableCaching)
        {
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => new FrameworkObject(value, false).DataContext;
    }
}