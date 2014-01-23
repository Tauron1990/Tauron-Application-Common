// The file IUiController.cs is part of Tauron.Application.Common.
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
// <copyright file="IUiController.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The shutdown mode.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.Contracts;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The shutdown mode.</summary>
    [PublicAPI]
    public enum ShutdownMode
    {
        /// <summary>The on last window close.</summary>
        OnLastWindowClose,

        /// <summary>The on main window close.</summary>
        OnMainWindowClose,

        /// <summary>The on explicit shutdown.</summary>
        OnExplicitShutdown,
    }

    /// <summary>The UIController interface.</summary>
    [PublicAPI]
    [ContractClass(typeof (UIControllerContract))]
    public interface IUIController
    {
        #region Public Properties

        /// <summary>Gets or sets the main window.</summary>
        /// <value>The main window.</value>
        IWindow MainWindow { get; set; }

        /// <summary>Gets or sets the shutdown mode.</summary>
        /// <value>The shutdown mode.</value>
        ShutdownMode ShutdownMode { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The run.
        /// </summary>
        /// <param name="window">
        ///     The window.
        /// </param>
        void Run(IWindow window);

        /// <summary>The shutdown.</summary>
        void Shutdown();

        #endregion
    }

    [ContractClassFor(typeof (IUIController))]
    internal abstract class UIControllerContract : IUIController
    {
        #region Public Properties

        /// <summary>Gets or sets the main window.</summary>
        /// <value>The main window.</value>
        /// <exception cref="NotImplementedException"></exception>
        public IWindow MainWindow
        {
            get { throw new NotImplementedException(); }

            set { throw new NotImplementedException(); }
        }

        /// <summary>Gets or sets the shutdown mode.</summary>
        /// <value>The shutdown mode.</value>
        /// <exception cref="NotImplementedException"></exception>
        public ShutdownMode ShutdownMode
        {
            get { throw new NotImplementedException(); }

            set { throw new NotImplementedException(); }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The run.
        /// </summary>
        /// <param name="window">
        ///     The window.
        /// </param>
        public void Run(IWindow window)
        {
            Contract.Requires<ArgumentNullException>(window != null, "window");
        }

        /// <summary>The shutdown.</summary>
        public void Shutdown()
        {
        }

        #endregion
    }
}