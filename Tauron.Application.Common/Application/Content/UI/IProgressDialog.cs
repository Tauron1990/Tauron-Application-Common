// The file IProgressDialog.cs is part of Tauron.Application.Common.
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
// <copyright file="IProgressDialog.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The progress style.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The progress style.</summary>
    [PublicAPI]
    public enum ProgressStyle
    {
        /// <summary>The none.</summary>
        None,

        /// <summary>The progress bar.</summary>
        ProgressBar,

        /// <summary>The marquee progress bar.</summary>
        MarqueeProgressBar,
    }

    /// <summary>The ProgressDialog interface.</summary>
    [PublicAPI]
    public interface IProgressDialog : IDisposable
    {
        #region Public Events

        /// <summary>The completed.</summary>
        event EventHandler Completed;

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the progress bar style.</summary>
        /// <value>The progress bar style.</value>
        ProgressStyle ProgressBarStyle { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>The start.</summary>
        void Start();

        #endregion
    }
}