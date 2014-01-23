// The file IImageHelper.cs is part of Tauron.Application.Common.Wpf.
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
// <copyright file="IImageHelper.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The ImageHelper interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.Contracts;
using System.Windows.Media;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The ImageHelper interface.</summary>
    [ContractClass(typeof (ImageHelperContracts))]
    public interface IImageHelper
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The convert.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <param name="assembly">
        ///     The assembly.
        /// </param>
        /// <returns>
        ///     The <see cref="ImageSource" />.
        /// </returns>
        [NotNull]
        ImageSource Convert([NotNull] Uri target, [NotNull] string assembly);

        /// <summary>
        ///     The convert.
        /// </summary>
        /// <param name="uri">
        ///     The uri.
        /// </param>
        /// <param name="assembly">
        ///     The assembly.
        /// </param>
        /// <returns>
        ///     The <see cref="ImageSource" />.
        /// </returns>
        [NotNull]
        ImageSource Convert([NotNull] string uri, [NotNull] string assembly);

        #endregion
    }

    [ContractClassFor(typeof (IImageHelper))]
    internal abstract class ImageHelperContracts : IImageHelper
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The convert.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <param name="assembly">
        ///     The assembly.
        /// </param>
        /// <returns>
        ///     The <see cref="ImageSource" />.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public ImageSource Convert(Uri target, string assembly)
        {
            Contract.Requires<ArgumentNullException>(target != null, "target");

            throw new NotImplementedException();
        }

        /// <summary>
        ///     The convert.
        /// </summary>
        /// <param name="uri">
        ///     The uri.
        /// </param>
        /// <param name="assembly">
        ///     The assembly.
        /// </param>
        /// <returns>
        ///     The <see cref="ImageSource" />.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public ImageSource Convert(string uri, string assembly)
        {
            Contract.Requires<ArgumentNullException>(uri != null, "uri");

            throw new NotImplementedException();
        }

        #endregion
    }
}