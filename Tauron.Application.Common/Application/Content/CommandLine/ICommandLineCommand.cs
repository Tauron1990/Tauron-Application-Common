// The file ICommandLineCommand.cs is part of Tauron.Application.Common.
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
// <copyright file="ICommandLineCommand.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The CommandLineCommand interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.Contracts;
using Tauron.Application.Ioc;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The CommandLineCommand interface.</summary>
    [PublicAPI]
    [ContractClass(typeof (CommandLineCommandContracts))]
    public interface ICommandLineCommand
    {
        #region Public Properties

        /// <summary>Gets the command name.</summary>
        /// <value>The command name.</value>
        string CommandName { get; }

        /// <summary>Gets the factory.</summary>
        /// <value>The factory.</value>
        IShellFactory Factory { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The execute.
        /// </summary>
        /// <param name="args">
        ///     The args.
        /// </param>
        /// <param name="container">
        ///     The container.
        /// </param>
        void Execute(string[] args, IContainer container);

        #endregion
    }

    [ContractClassFor(typeof (ICommandLineCommand))]
    internal abstract class CommandLineCommandContracts : ICommandLineCommand
    {
        #region Public Properties

        /// <summary>Gets the command name.</summary>
        /// <value>The command name.</value>
        public string CommandName
        {
            get
            {
                Contract.Ensures(Contract.Result<string>() != null);

                return null;
            }
        }

        /// <summary>Gets the factory.</summary>
        /// <value>The factory.</value>
        public IShellFactory Factory { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The execute.
        /// </summary>
        /// <param name="args">
        ///     The args.
        /// </param>
        /// <param name="container">
        ///     The container.
        /// </param>
        public void Execute(string[] args, IContainer container)
        {
            Contract.Requires<ArgumentNullException>(args != null, "args");
            Contract.Requires<ArgumentNullException>(container != null, "container");
        }

        #endregion
    }
}