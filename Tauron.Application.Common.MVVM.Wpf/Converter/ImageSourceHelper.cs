﻿using System;
using System.Windows.Markup;
using JetBrains.Annotations;
using NLog;

namespace Tauron.Application.Converter
{
    internal static class ImageSourceHelper
    {
        private static Logger GetLogger() => LogManager.GetLogger(nameof(ImageSourceHelper));

        public static bool Enter([CanBeNull] string imageSource, [NotNull] IServiceProvider provider)
        {
            if (imageSource != null) return false;

            GetLogger().Warn("InmageSource are null. {0}",
                provider.GetService<IProvideValueTarget>().TargetObject);


            return true;
        }

        public static bool Exit([NotNull] string imageSource, bool isNull)
        {
            if (!isNull) return false;

            GetLogger().Warn("Inmage not Found: {0}.", imageSource);

            return true;
        }

        [NotNull]
        public static string ResolveAssembly([NotNull] string assembly, [NotNull] IServiceProvider provider)
        {
            if (assembly != "this") return assembly;

            var target = provider.GetService<IProvideValueTarget>();
            return target == null ? assembly : target.TargetObject.GetType().Assembly.FullName;
        }
    }
}