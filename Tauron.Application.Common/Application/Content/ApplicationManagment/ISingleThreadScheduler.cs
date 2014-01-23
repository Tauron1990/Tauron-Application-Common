// The file ISingleThreadScheduler.cs is part of Tauron.Application.Common.
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
// <copyright file="ISingleThreadScheduler.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The SingleThreadScheduler interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.Contracts;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The SingleThreadScheduler interface.</summary>
    [PublicAPI]
    [ContractClass(typeof (SingleThreadSchedulerContracts))]
    public interface ISingleThreadScheduler
    {
        #region Public Properties

        /// <summary>Gets or sets a value indicating whether is background.</summary>
        /// <value>The is background.</value>
        bool IsBackground { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The queue.
        /// </summary>
        /// <param name="task">
        ///     The task.
        /// </param>
        void Queue(Action task);

        #endregion
    }

    [ContractClassFor(typeof (ISingleThreadScheduler))]
    internal abstract class SingleThreadSchedulerContracts : ISingleThreadScheduler
    {
        #region Public Properties

        /// <summary>Gets or sets a value indicating whether is background.</summary>
        /// <value>The is background.</value>
        public bool IsBackground { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The queue.
        /// </summary>
        /// <param name="task">
        ///     The task.
        /// </param>
        public void Queue(Action task)
        {
            Contract.Requires<ArgumentNullException>(task != null, "task");
        }

        #endregion
    }
}