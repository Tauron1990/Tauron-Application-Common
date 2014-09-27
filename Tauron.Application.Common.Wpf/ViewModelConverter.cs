using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using Tauron.Application.Converter;
using Tauron.Application.Models;
using Tauron.Application.Views;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application
{
    public sealed class ViewModelConverterExtension : ValueConverterFactoryBase
    {
        public bool EnableCaching { get; set; }

        public ViewModelConverterExtension()
        {
            EnableCaching = true;
        }

        protected override IValueConverter Create()
        {
            return new ViewModelConverter(EnableCaching);
        }
    }

    public class ViewModelConverter : IValueConverter
    {
        private readonly bool _enableCaching;
        private readonly Dictionary<string, object> _cache;
        
        public ViewModelConverter(bool enableCaching)
        {
            _enableCaching = enableCaching;

            if(enableCaching)
                _cache = new Dictionary<string, object>();
        }

        [CanBeNull]
        public object Convert([NotNull] object value, [NotNull] Type targetType, [NotNull] object parameter, [NotNull] CultureInfo culture)
        {
            var model = value as ViewModelBase;
            if (model == null) return value;

            var manager = ViewManager.Manager;

            if (!_enableCaching) return manager.CreateViewForModel(model);

            var name = manager.GetName(model);
            if (string.IsNullOrEmpty(name)) return value;

            object view;
            if (_cache.TryGetValue(name, out view))
                return view;

            view = manager.CreateViewForModel(model);
            _cache[name] = view;
            return view;
        }

        [CanBeNull]
        public object ConvertBack([NotNull] object value, [NotNull] Type targetType, [NotNull] object parameter, [NotNull] CultureInfo culture)
        {
            return new FrameworkObject(value, false).DataContext;
        }
    }
}