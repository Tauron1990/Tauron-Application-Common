// The file ClipboardManager.cs is part of Tauron.Application.Common.
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
// <copyright file="ClipboardManager.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The clipboard manager.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Tauron.Application.Ioc;

#endregion

namespace Tauron.Application.Implement
{
    /// <summary>The clipboard manager.</summary>
    [Export(typeof (IClipboardManager))]
    public class ClipboardManager : IClipboardManager
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
            return new ClipboardViewer(target, registerForClose, performInitialization);
        }

        #endregion
    }
}