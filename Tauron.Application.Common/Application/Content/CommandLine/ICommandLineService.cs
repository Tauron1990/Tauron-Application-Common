// The file ICommandLineService.cs is part of Tauron.Application.Common.
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
// <copyright file="ICommandLineService.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The CommandLineService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The CommandLineService interface.</summary>
    [PublicAPI]
    [ContractClass(typeof (CommandLineServiceContracts))]
    public interface ICommandLineService
    {
        #region Public Properties

        /// <summary>Gets the commands.</summary>
        /// <value>The commands.</value>
        IEnumerable<ICommandLineCommand> Commands { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The add.
        /// </summary>
        /// <param name="command">
        ///     The command.
        /// </param>
        void Add(ICommandLineCommand command);

        #endregion
    }

    [ContractClassFor(typeof (ICommandLineService))]
    internal abstract class CommandLineServiceContracts : ICommandLineService
    {
        #region Public Properties

        /// <summary>Gets the commands.</summary>
        /// <value>The commands.</value>
        public IEnumerable<ICommandLineCommand> Commands
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<ICommandLineCommand>>() != null);

                return null;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The add.
        /// </summary>
        /// <param name="command">
        ///     The command.
        /// </param>
        public void Add(ICommandLineCommand command)
        {
            Contract.Requires<ArgumentNullException>(command != null, "command");
        }

        #endregion
    }
}