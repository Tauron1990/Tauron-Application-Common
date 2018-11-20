using System;
using System.Collections.Generic;
using System.Globalization;
using JetBrains.Annotations;
using Tauron.Application.Models;
using Tauron.Application.Views;

namespace Tauron.Application
{

    public class ViewModelConverter
    {
        private readonly Dictionary<string, object> _cache;
        private readonly bool _enableCaching;

        public ViewModelConverter(bool enableCaching)
        {
            _enableCaching = enableCaching;

            if (enableCaching)
                _cache = new Dictionary<string, object>();
        }

        [CanBeNull]
        public object Convert([NotNull] object value, [NotNull] Type targetType, [NotNull] object parameter, [NotNull] CultureInfo culture)
        {
            if (!(value is ViewModelBase model)) return value;

            var manager = ViewManager.Manager;

            if (!_enableCaching) return manager.CreateViewForModel(model);

            var name = manager.GetName(model);
            if (string.IsNullOrEmpty(name)) return value;

            if (_cache.TryGetValue(name, out var view))
                return view;

            view = manager.CreateViewForModel(model);
            _cache[name] = view;
            return view;
        }

        [CanBeNull]
        public virtual object ConvertBack([NotNull] object value, [NotNull] Type targetType, [NotNull] object parameter, [NotNull] CultureInfo culture) => throw new NotSupportedException();
    }
}