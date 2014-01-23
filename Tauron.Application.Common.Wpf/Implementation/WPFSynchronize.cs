// The file WPFSynchronize.cs is part of Tauron.Application.Common.Wpf.
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
// <copyright file="WPFSynchronize.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The wpf synchronize.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using System.Windows.Threading;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Implementation
{
    /// <summary>The wpf synchronize.</summary>
    [PublicAPI]
    public class WPFSynchronize : IUISynchronize
    {
        #region Fields

        private readonly Dispatcher dispatcher;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="WPFSynchronize" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="WPFSynchronize" /> Klasse.
        /// </summary>
        /// <param name="dispatcher">
        ///     The dispatcher.
        /// </param>
        public WPFSynchronize(Dispatcher dispatcher)
        {
            Contract.Requires<ArgumentNullException>(dispatcher != null, "dispatcher");

            this.dispatcher = dispatcher;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The begin invoke.
        /// </summary>
        /// <param name="action">
        ///     The action.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public Task BeginInvoke(Action action)
        {
            return dispatcher.BeginInvoke(action).Task;
        }

        /// <summary>
        ///     The invoke.
        /// </summary>
        /// <param name="action">
        ///     The action.
        /// </param>
        public void Invoke(Action action)
        {
            dispatcher.Invoke(action);
        }

        /// <summary>
        ///     The invoke.
        /// </summary>
        /// <param name="action">
        ///     The action.
        /// </param>
        /// <typeparam name="TReturn">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="TReturn" />.
        /// </returns>
        public TReturn Invoke<TReturn>(Func<TReturn> action)
        {
            return dispatcher.Invoke(action);
        }

        #endregion
    }
}