// The file BrushValueConverter.cs is part of Tauron.Application.Common.Wpf.
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
// <copyright file="BrushValueConverter.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The brush value converter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Windows.Data;
using System.Windows.Media;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Converter
{
    /// <summary>
    ///     The brush value converter.
    /// </summary>
    public sealed class BrushValueConverter : ValueConverterFactoryBase
    {
        #region Methods

        /// <summary>
        ///     The create.
        /// </summary>
        /// <returns>
        ///     The <see cref="IValueConverter" />.
        /// </returns>
        protected override IValueConverter Create()
        {
            return new Converter();
        }

        #endregion

        private class Converter : ValueConverterBase<string, Brush>
        {
            #region Static Fields

            private static readonly BrushConverter ConverterImpl = new BrushConverter();

            #endregion

            #region Methods

            /// <summary>
            ///     The convert.
            /// </summary>
            /// <param name="value">
            ///     The value.
            /// </param>
            /// <returns>
            ///     The <see cref="Brush" />.
            /// </returns>
            [CanBeNull]
            protected override Brush Convert([NotNull] string value)
            {
                return ConverterImpl.ConvertFrom(value) as Brush;
            }

            #endregion
        }
    }
}