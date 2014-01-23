// The file ISingleInstanceApp.cs is part of Tauron.Application.Common.
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
// <copyright file="ISingleInstanceApp.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The SingleInstanceApp interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The SingleInstanceApp interface.</summary>
    [PublicAPI]
    [ContractClass(typeof (SingleInstanceAppContracts))]
    public interface ISingleInstanceApp
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The signal external command line args.
        /// </summary>
        /// <param name="args">
        ///     The args.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        bool SignalExternalCommandLineArgs(IList<string> args);

        #endregion
    }

    [ContractClassFor(typeof (ISingleInstanceApp))]
    internal abstract class SingleInstanceAppContracts : ISingleInstanceApp
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The signal external command line args.
        /// </summary>
        /// <param name="args">
        ///     The args.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            Contract.Requires<ArgumentNullException>(args != null, "args");
            return false;
        }

        #endregion
    }
}