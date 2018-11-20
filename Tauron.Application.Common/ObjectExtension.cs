using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using JetBrains.Annotations;

namespace Tauron
{
    [SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32")]
    [PublicAPI]
    public enum RoundType : short
    {
        None = 0,

        Hour = 60,

        HalfHour = 30,

        QuaterHour = 15
    }

    [PublicAPI]
    public static class ObjectExtension
    {
        [CanBeNull]
        public static T As<T>([CanBeNull] this object value) where T : class => value as T;

        public static T SafeCast<T>([CanBeNull] this object value)
        {
            if (value == null) return default;

            return (T) value;
        }

        public static DateTime CutSecond(this DateTime source) => new DateTime(source.Year, source.Month, source.Day, source.Hour, source.Minute, 0);

        public static T GetService<T>([NotNull] this IServiceProvider provider)
        {
            if (provider == null) throw new ArgumentNullException(nameof(provider));
            var temp = provider.GetService(typeof(T));
            if (temp == null) return default;

            return (T) temp;
        }

        public static bool IsAlive<TType>([NotNull] this WeakReference<TType> reference) where TType : class
        {
            if (reference == null) throw new ArgumentNullException(nameof(reference));
            return reference.TryGetTarget(out _);
        }

        public static DateTime Round(this DateTime source, RoundType type)
        {
            if (!Enum.IsDefined(typeof(RoundType), type))
                throw new InvalidEnumArgumentException(nameof(type), (int) type, typeof(RoundType));
            if (type == RoundType.None)
                throw new ArgumentNullException(nameof(type));

            return Round(source, (double) type);
        }

        public static DateTime Round(this DateTime source, double type)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (type == 0)
                throw new ArgumentNullException(nameof(type));

            var result = source;

            var minutes = type;

            Math.DivRem(source.Minute, (int) minutes, out var modulo);

            if (modulo <= 0) return result;

            result = modulo >= minutes / 2 ? source.AddMinutes(minutes - modulo) : source.AddMinutes(modulo * -1);

            result = result.AddSeconds(source.Second * -1);

            return result;
        }

        [NotNull]
        [StringFormatMethod("format")]
        public static string SFormat([NotNull] this string format, [NotNull] params object[] args)
        {
            if (format == null) throw new ArgumentNullException(nameof(format));
            if (args == null) throw new ArgumentNullException(nameof(args));
            return string.Format(CultureInfo.InvariantCulture, format, args);
        }

        [CanBeNull]
        public static TType TypedTarget<TType>([NotNull] this WeakReference<TType> reference) where TType : class
        {
            if (reference == null) throw new ArgumentNullException(nameof(reference));
            return reference.TryGetTarget(out var obj) ? obj : null;
        }
    }
}