// The file CommandLineCommandBase.cs is part of Tauron.Application.Common.
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

using System;
using System.Diagnostics.Contracts;
using Tauron.Application.Ioc;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The command line command base.</summary>
    public abstract class CommandLineCommandBase : ICommandLineCommand
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="CommandLineCommandBase" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="CommandLineCommandBase" /> Klasse.
        ///     Initializes a new instance of the <see cref="CommandLineCommandBase" /> class.
        /// </summary>
        /// <param name="comandName">
        ///     The comand name.
        /// </param>
        protected CommandLineCommandBase([NotNull] string comandName)
        {
            Contract.Requires<ArgumentNullException>(comandName != null, "comandName");

            CommandName = comandName;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the command name.</summary>
        /// <value>The command name.</value>
        [NotNull]
        public string CommandName { get; private set; }

        /// <summary>Gets or sets the factory.</summary>
        /// <value>The factory.</value>
        [CanBeNull]
        public IShellFactory Factory { get; protected set; }

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
        public virtual void Execute([NotNull] string[] args, [NotNull] IContainer container)
        {
            CommonConstants.LogCommon(false, "Command: {0} Empty Executet", CommandName);
        }

        #endregion
    }
}