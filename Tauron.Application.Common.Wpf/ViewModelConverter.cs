using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Tauron.Application.Converter;
using Tauron.Application.Models;
using Tauron.Application.Views;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application
{
    public sealed class ViewModelConverterExtension : ValueConverterFactoryBase
    {
        protected override IValueConverter Create()
        {
            return new ViewModelConverter();
        }
    }

    public class ViewModelConverter : IValueConverter
    {
        [CanBeNull]
        public object Convert([NotNull] object value, [NotNull] Type targetType, [NotNull] object parameter, [NotNull] CultureInfo culture)
        {
            var model = value as ViewModelBase;
            return model == null ? value : ViewManager.Manager.CreateViewForModel(model);
        }

        [CanBeNull]
        public object ConvertBack([NotNull] object value, [NotNull] Type targetType, [NotNull] object parameter, [NotNull] CultureInfo culture)
        {
            var view = value as FrameworkElement;
            return view == null ? value : view.DataContext;
        }
    }
}