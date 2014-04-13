// The file CommandLineProcessor.cs is part of Tauron.Application.Common.
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
// <copyright file="CommandLineProcessor.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The command line processor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Implement
{
    /// <summary>The command line processor.</summary>
    [PublicAPI]
    public class CommandLineProcessor
    {
        #region Fields

        /// <summary>The _application.</summary>
        private readonly CommonApplication _application;

        /// <summary>The _commands.</summary>
        private List<Command> _commands = new List<Command>();

        /// <summary>The _factory.</summary>
        private IShellFactory _factory;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="CommandLineProcessor" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="CommandLineProcessor" /> Klasse.
        ///     Initializes a new instance of the <see cref="CommandLineProcessor" /> class.
        /// </summary>
        /// <param name="application">
        ///     The application.
        /// </param>
        public CommandLineProcessor([NotNull] CommonApplication application)
        {
            Contract.Requires<ArgumentNullException>(application != null, "application");
            _application = application;
            ParseCommandLine();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The create shell view.</summary>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        [CanBeNull]
        public object CreateShellView()
        {
            SelectViewFacotry();
            return _factory == null ? null : _factory.CreateView();
        }

        /// <summary>The execute commands.</summary>
        public void ExecuteCommands()
        {
            foreach (Command fileCommand in
                _commands.Where(com => com.Name == "FileCommand").ToArray())
            {
                var commandPrcessor = _application.Container.Resolve<IFileCommand>(null, true);
                if (commandPrcessor == null) break;

                string file = fileCommand.Parms[0];

                commandPrcessor.ProcessFile(file);
                _factory = commandPrcessor.ProvideFactory();
            }

            foreach (ICommandLineCommand command in
                _application.Container.Resolve<ICommandLineService>().Commands)
            {
                ICommandLineCommand command1 = command;
                Command temp = _commands.FirstOrDefault(arg => arg.Name == command1.CommandName);
                if (temp == null) continue;

                command.Execute(temp.Parms.ToArray(), _application.Container);
            }

            _commands = null;
        }

        #endregion

        #region Methods

        /// <summary>The parse command line.</summary>
        private void ParseCommandLine()
        {
            Contract.Requires(_application != null);

            Command current = null;
            bool first = true;
            foreach (string arg in _application.GetArgs())
            {
                if (first)
                {
                    first = false;
                    continue;
                }

                if (current == null && arg.ExisFile())
                {
                    var temp = new Command("FileCommand");
                    temp.Parms.Add(arg);
                    _commands.Add(temp);
                }

                if (arg.StartsWith("-", StringComparison.Ordinal))
                {
                    if (current != null) _commands.Add(current);

                    current = new Command(arg.TrimStart('-'));
                }
                else if (current != null) current.Parms.Add(arg);
            }

            if (current != null && !_commands.Contains(current)) _commands.Add(current);
        }

        /// <summary>The select view facotry.</summary>
        private void SelectViewFacotry()
        {
            Contract.Requires(_application != null);

            if (_factory != null) return;

            foreach (ICommandLineCommand command in
                _application.Container.Resolve<ICommandLineService>()
                           .Commands.Where(
                               command =>
                               command.Factory != null &&
                               _commands.Any(com => com.Name == command.CommandName))
                ) _factory = command.Factory;
        }

        #endregion

        /// <summary>The command.</summary>
        private class Command
        {
            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="Command" /> class.
            ///     Initialisiert eine neue Instanz der <see cref="Command" /> Klasse.
            ///     Initializes a new instance of the <see cref="Command" /> class.
            /// </summary>
            /// <param name="name">
            ///     The name.
            /// </param>
            public Command([NotNull] string name)
            {
                Contract.Requires<ArgumentNullException>(name != null, "name");

                Name = name;
                Parms = new List<string>();
            }

            #endregion

            #region Public Properties

            /// <summary>Gets the name.</summary>
            /// <value>The name.</value>
            [NotNull]
            public string Name { get; private set; }

            /// <summary>Gets the parms.</summary>
            /// <value>The parms.</value>
            [NotNull]
            public List<string> Parms { get; private set; }

            #endregion
        }
    }
}