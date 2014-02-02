// The file ITabWorkspace.cs is part of Tauron.Application.Common.Wpf.
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
// <copyright file="ITabWorkspace.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The TabWorkspace interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The TabWorkspace interface.</summary>
    [PublicAPI]
    public interface ITabWorkspace
    {
        #region Public Events

        /// <summary>The close.</summary>
        [SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        event Action<ITabWorkspace> Close;

        #endregion

        #region Public Properties

        /// <summary>Gets or sets a value indicating whether can close.</summary>
        bool CanClose { get; set; }

        /// <summary>Gets the close workspace.</summary>
        [NotNull]
        ICommand CloseWorkspace { get; }

        /// <summary>Gets or sets the title.</summary>
        [NotNull]
        string Title { get; set; }

        #endregion

        #region Public Methods and Operators

        void OnClose();

        void OnActivate();

        void OnDeactivate();

        #endregion
    }
}