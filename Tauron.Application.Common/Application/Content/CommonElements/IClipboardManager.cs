// The file IClipboardManager.cs is part of Tauron.Application.Common.
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
//  along with Tauron.Application.Common If not, see <http://www.gnu.org/licenses/>.

#region

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IClipboardManager.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The ClipboardManager interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.Contracts;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The ClipboardManager interface.</summary>
    [ContractClass(typeof (ClipboardManagerContracts))]
    public interface IClipboardManager
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The create viewer.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <param name="registerForClose">
        ///     The register for close.
        /// </param>
        /// <param name="performInitialization">
        ///     The perform initialization.
        /// </param>
        /// <returns>
        ///     The <see cref="ClipboardViewer" />.
        /// </returns>
        [NotNull]
        ClipboardViewer CreateViewer([NotNull] IWindow target, bool registerForClose, bool performInitialization);

        #endregion
    }

    [ContractClassFor(typeof (IClipboardManager))]
    internal abstract class ClipboardManagerContracts : IClipboardManager
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The create viewer.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <param name="registerForClose">
        ///     The register for close.
        /// </param>
        /// <param name="performInitialization">
        ///     The perform initialization.
        /// </param>
        /// <returns>
        ///     The <see cref="ClipboardViewer" />.
        /// </returns>
        public ClipboardViewer CreateViewer(IWindow target, bool registerForClose, bool performInitialization)
        {
            Contract.Requires<ArgumentNullException>(target != null, "target");
            Contract.Ensures(Contract.Result<ClipboardViewer>() != null);

            return null;
        }

        #endregion
    }
}