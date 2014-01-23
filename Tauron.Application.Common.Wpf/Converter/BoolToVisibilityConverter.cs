// The file BoolToVisibilityConverter.cs is part of Tauron.Application.Common.Wpf.
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
// <copyright file="BoolToVisibilityConverter.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The bool to visibility converter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Windows;
using System.Windows.Data;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Converter
{
    /// <summary>The bool to visibility converter.</summary>
    [PublicAPI]
    public class BoolToVisibilityConverter : ValueConverterFactoryBase
    {
        #region Public Properties

        /// <summary>Gets or sets a value indicating whether is hidden.</summary>
        public bool IsHidden { get; set; }

        /// <summary>Gets or sets a value indicating whether reverse.</summary>
        public bool Reverse { get; set; }

        #endregion

        #region Methods

        /// <summary>The create.</summary>
        /// <returns>
        ///     The <see cref="IValueConverter" />.
        /// </returns>
        protected override IValueConverter Create()
        {
            return new Converter(IsHidden, Reverse);
        }

        #endregion

        private class Converter : ValueConverterBase<bool, Visibility>
        {
            #region Fields

            private readonly bool isHidden;

            private readonly bool reverse;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="Converter" /> class.
            ///     Initialisiert eine neue Instanz der <see cref="Converter" /> Klasse.
            /// </summary>
            /// <param name="isHidden">
            ///     The is hidden.
            /// </param>
            /// <param name="reverse">
            ///     The reverse.
            /// </param>
            public Converter(bool isHidden, bool reverse)
            {
                this.isHidden = isHidden;
                this.reverse = reverse;
            }

            #endregion

            #region Properties

            /// <summary>Gets a value indicating whether can convert back.</summary>
            protected override bool CanConvertBack
            {
                get { return true; }
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
            ///     The <see cref="Visibility" />.
            /// </returns>
            protected override Visibility Convert(bool value)
            {
                if (reverse) value = !value;

                if (value) return Visibility.Visible;

                return isHidden ? Visibility.Hidden : Visibility.Collapsed;
            }

            /// <summary>
            ///     The convert back.
            /// </summary>
            /// <param name="value">
            ///     The value.
            /// </param>
            /// <returns>
            ///     The <see cref="bool" />.
            /// </returns>
            protected override bool ConvertBack(Visibility value)
            {
                bool result;
                switch (value)
                {
                    case Visibility.Collapsed:
                    case Visibility.Hidden:
                        result = false;
                        break;
                    case Visibility.Visible:
                        result = true;
                        break;
                    default:
                        result = false;
                        break;
                }

                if (reverse) result = !result;

                return result;
            }

            #endregion
        }
    }
}