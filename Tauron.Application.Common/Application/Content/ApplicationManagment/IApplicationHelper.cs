// The file IApplicationHelper.cs is part of Tauron.Application.Common.
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
// <copyright file="IApplicationHelper.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The ApplicationHelper interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Threading;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The ApplicationHelper interface.</summary>
    [ContractClass(typeof (ApplicationHelperContracts))]
    public interface IApplicationHelper
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The create ui thread.
        /// </summary>
        /// <param name="start">
        ///     The start.
        /// </param>
        /// <returns>
        ///     The <see cref="Thread" />.
        /// </returns>
        [NotNull,PublicAPI]
        Thread CreateUIThread([NotNull] ThreadStart start);

        /// <summary>The run anonymous application.</summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>
        ///     The <see cref="IWindow" />.
        /// </returns>
        [NotNull,SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        [PublicAPI]
        IWindow RunAnonymousApplication<T>() where T : class, IWindow;

        /// <summary>
        ///     The run anonymous application.
        /// </summary>
        /// <param name="window">
        ///     The window.
        /// </param>
        [PublicAPI]
        void RunAnonymousApplication([NotNull] IWindow window);

        /// <summary>
        ///     The run ui thread.
        /// </summary>
        /// <param name="start">
        ///     The start.
        /// </param>
        [PublicAPI]
        void RunUIThread([NotNull] ThreadStart start);

        #endregion
    }

    /// <summary>The application helper contracts.</summary>
    [ContractClassFor(typeof (IApplicationHelper))]
    internal abstract class ApplicationHelperContracts : IApplicationHelper
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The create ui thread.
        /// </summary>
        /// <param name="start">
        ///     The start.
        /// </param>
        /// <returns>
        ///     The <see cref="Thread" />.
        /// </returns>
        public Thread CreateUIThread(ThreadStart start)
        {
            Contract.Requires<ArgumentNullException>(start != null, "start");
            Contract.Ensures(Contract.Result<Thread>() != null);
            return null;
        }

        /// <summary>The run anonymous application.</summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>
        ///     The <see cref="IWindow" />.
        /// </returns>
        public IWindow RunAnonymousApplication<T>() where T : class, IWindow
        {
            Contract.Ensures(Contract.Result<IWindow>() != null);
            return null;
        }

        /// <summary>
        ///     The run anonymous application.
        /// </summary>
        /// <param name="window">
        ///     The window.
        /// </param>
        public void RunAnonymousApplication(IWindow window)
        {
            Contract.Requires<ArgumentNullException>(window != null, "window");
        }

        /// <summary>
        ///     The run ui thread.
        /// </summary>
        /// <param name="start">
        ///     The start.
        /// </param>
        public void RunUIThread(ThreadStart start)
        {
            Contract.Requires<ArgumentNullException>(start != null, "start");
        }

        #endregion
    }
}