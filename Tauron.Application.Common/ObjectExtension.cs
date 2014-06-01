#region

using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron
{
    /// <summary>The Rounding types.</summary>
    [SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32")]
    [PublicAPI]
    public enum RoundType : short
    {
        /// <summary>The none.</summary>
        None = 0,

        /// <summary>Complete Hour.</summary>
        Hour = 60,

        /// <summary>Half an Hour.</summary>
        HalfHour = 30,

        /// <summary>15 Miniutes.</summary>
        QuaterHour = 15,
    }

    /// <summary>The object extension.</summary>
    [PublicAPI]
    public static class ObjectExtension
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The as.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="T" />.
        /// </returns>
        [CanBeNull]
        public static T As<T>([CanBeNull] this object value) where T : class
        {
            if (value == null) return default(T);

            return value as T;
        }

        /// <summary>
        ///     The cast obj.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="T" />.
        /// </returns>
        public static T CastObj<T>([CanBeNull] this object value)
        {
            if (value == null) return default(T);

            return (T) value;
        }

        /// <summary>
        ///     The cut second.
        /// </summary>
        /// <param name="source">
        ///     The source.
        /// </param>
        /// <returns>
        ///     The <see cref="DateTime" />.
        /// </returns>
        public static DateTime CutSecond(this DateTime source)
        {
            return new DateTime(source.Year, source.Month, source.Day, source.Hour, source.Minute, 0);
        }

        public static T GetService<T>([NotNull] this IServiceProvider provider)
        {
            Contract.Requires<ArgumentNullException>(provider != null, "provider");

            object temp = provider.GetService(typeof (T));
            if (temp == null) return default(T);

            return (T) temp;
        }

        /// <summary>
        ///     The is alive.
        /// </summary>
        /// <param name="reference">
        ///     The reference.
        /// </param>
        /// <typeparam name="TType">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool IsAlive<TType>([NotNull] this WeakReference<TType> reference) where TType : class
        {
            Contract.Requires<ArgumentNullException>(reference != null, "reference");

            TType o;
            return reference.TryGetTarget(out o);
        }

        /// <summary>
        ///     Roundes the given DateTime to the given Minutes Pattern (15,30,60...).
        /// </summary>
        /// <param name="source">
        ///     DateTime which should be rounded.
        /// </param>
        /// <param name="type">
        ///     The Roundtype.
        /// </param>
        /// <returns>
        ///     The <see cref="DateTime" />.
        /// </returns>
        public static DateTime Round(this DateTime source, RoundType type)
        {
            Contract.Requires<ArgumentException>((double) type != 0, "type");

            return Round(source, (double) type);
        }

        /// <summary>
        ///     Roundes the given DateTime to the given Minutes Pattern (15,30,60...).
        /// </summary>
        /// <param name="source">
        ///     DateTime which should be rounded.
        /// </param>
        /// <param name="type">
        ///     The Roundtype.
        /// </param>
        /// <returns>
        ///     The <see cref="DateTime" />.
        /// </returns>
        public static DateTime Round(this DateTime source, double type)
        {
            Contract.Requires<ArgumentException>((int) type != 0, "type");

            DateTime result = source;

            double minutes = type;
            int modulo;

            Math.DivRem(source.Minute, (int) minutes, out modulo);

            if (modulo <= 0) return result;

            result = modulo >= (minutes/2) ? source.AddMinutes(minutes - modulo) : source.AddMinutes(modulo*-1);

            result = result.AddSeconds(source.Second*-1);

            return result;
        }

        /// <summary>
        ///     The s format.
        /// </summary>
        /// <param name="format">
        ///     The format.
        /// </param>
        /// <param name="args">
        ///     The args.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        [NotNull, StringFormatMethod("format")]
        public static string SFormat([NotNull] this string format, [NotNull] params object[] args)
        {
            Contract.Requires<ArgumentNullException>(format != null, "format");
            Contract.Requires<ArgumentNullException>(args != null, "args");

            return string.Format(CultureInfo.InvariantCulture, format, args);
        }

        /// <summary>
        ///     The typed target.
        /// </summary>
        /// <param name="reference">
        ///     The reference.
        /// </param>
        /// <typeparam name="TType">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="TType" />.
        /// </returns>
        [CanBeNull]
        public static TType TypedTarget<TType>([NotNull] this WeakReference<TType> reference) where TType : class
        {
            Contract.Requires<ArgumentNullException>(reference != null, "reference");

            TType obj;
            return reference.TryGetTarget(out obj) ? obj : null;
        }

        #endregion
    }
}