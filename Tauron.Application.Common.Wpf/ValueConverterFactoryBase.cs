// The file ValueConverterFactoryBase.cs is part of Tauron.Application.Common.Wpf.
// 
// CoreEngine is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// CoreEngine is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//  
// You should have received a copy of the GNU General Public License
//  along with Tauron.Application.Common.Wpf If not, see <http://www.gnu.org/licenses/>.

#region

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueConverterFactoryBase.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The value converter factory base.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The value converter factory base.</summary>
    [MarkupExtensionReturnType(typeof (IValueConverter))]
    [DebuggerNonUserCode]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public abstract class ValueConverterFactoryBase : MarkupExtension
    {
        #region Public Properties

        /// <summary>Gets or sets the service provider.</summary>
        [CanBeNull]
        public IServiceProvider ServiceProvider { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gibt bei der Implementierung in einer abgeleiteten Klasse ein Objekt zurück, das als Wert der Zieleigenschaft für
        ///     die Markuperweiterung festgelegt wird.
        /// </summary>
        /// <returns>
        ///     Der Objektwert, der für die Eigenschaft festgelegt werden soll, für die die Erweiterung angewendet wird.
        /// </returns>
        /// <param name="serviceProvider">
        ///     Objekt, das Dienste für die Markuperweiterung bereitstellen kann.
        /// </param>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;

            return Create();
        }

        #endregion

        #region Methods

        /// <summary>The create.</summary>
        /// <returns>
        ///     The <see cref="IValueConverter" />.
        /// </returns>
        [NotNull]
        protected abstract IValueConverter Create();

        #endregion

        /// <summary>
        ///     The string converter base.
        /// </summary>
        /// <typeparam name="TSource">
        /// </typeparam>
        protected abstract class StringConverterBase<TSource> : ValueConverterBase<TSource, string>
        {
        }

        /// <summary>
        ///     The value converter base.
        /// </summary>
        /// <typeparam name="TSource">
        /// </typeparam>
        /// <typeparam name="TDest">
        /// </typeparam>
        protected abstract class ValueConverterBase<TSource, TDest> : IValueConverter
        {
            #region Properties

            /// <summary>Gets a value indicating whether can convert back.</summary>
            protected virtual bool CanConvertBack
            {
                get { return false; }
            }

            #endregion

            #region Public Methods and Operators

            /// <summary>
            ///     Konvertiert einen Wert.
            /// </summary>
            /// <returns>
            ///     Ein konvertierter Wert.Wenn die Methode null zurückgibt, wird der gültige NULL-Wert verwendet.
            /// </returns>
            /// <param name="value">
            ///     Der von der Bindungsquelle erzeugte Wert.
            /// </param>
            /// <param name="targetType">
            ///     Der Typ der Bindungsziel-Eigenschaft.
            /// </param>
            /// <param name="parameter">
            ///     Der zu verwendende Konverterparameter.
            /// </param>
            /// <param name="culture">
            ///     Die im Konverter zu verwendende Kultur.
            /// </param>
            [CanBeNull]
            public virtual object Convert([NotNull] object value, [NotNull] Type targetType, [NotNull] object parameter, [NotNull] CultureInfo culture)
            {
                if (!(value is TSource)) return null;

                return Convert((TSource) value);
            }

            /// <summary>
            ///     Konvertiert einen Wert.
            /// </summary>
            /// <returns>
            ///     Ein konvertierter Wert.Wenn die Methode null zurückgibt, wird der gültige NULL-Wert verwendet.
            /// </returns>
            /// <param name="value">
            ///     Der Wert, der vom Bindungsziel erzeugt wird.
            /// </param>
            /// <param name="targetType">
            ///     Der Typ, in den konvertiert werden soll.
            /// </param>
            /// <param name="parameter">
            ///     Der zu verwendende Konverterparameter.
            /// </param>
            /// <param name="culture">
            ///     Die im Konverter zu verwendende Kultur.
            /// </param>
            [CanBeNull]
            public virtual object ConvertBack([NotNull] object value, [NotNull] Type targetType, [NotNull] object parameter, [NotNull] CultureInfo culture)
            {
                if (!CanConvertBack || !(value is TDest)) return null;

                return ConvertBack((TDest) value);
            }

            #endregion

            #region Methods

            /// <summary>
            ///     The convert.
            /// </summary>
            /// <param name="value">
            ///     The value.
            /// </param>
            /// <returns>
            ///     The <see cref="TDest" />.
            /// </returns>
            protected abstract TDest Convert(TSource value);

            /// <summary>
            ///     The convert back.
            /// </summary>
            /// <param name="value">
            ///     The value.
            /// </param>
            /// <returns>
            ///     The <see cref="TSource" />.
            /// </returns>
            protected virtual TSource ConvertBack(TDest value)
            {
                return default(TSource);
            }

            #endregion
        }
    }
}