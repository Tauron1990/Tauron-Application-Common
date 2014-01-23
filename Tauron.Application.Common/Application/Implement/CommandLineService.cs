// The file CommandLineService.cs is part of Tauron.Application.Common.
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
// <copyright file="CommandLineService.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The command line service.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using Tauron.Application.Ioc;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Implement
{
    /// <summary>The command line service.</summary>
    [Export(typeof (ICommandLineService))]
    public class CommandLineService : ICommandLineService
    {
        #region Fields

        /// <summary>The _commands.</summary>
        [Inject] private readonly List<ICommandLineCommand> _commands = new List<ICommandLineCommand>();

        #endregion

        #region Public Properties

        /// <summary>Gets the commands.</summary>
        /// <value>The commands.</value>
        [NotNull]
        public IEnumerable<ICommandLineCommand> Commands
        {
            get { return _commands.AsReadOnly(); }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The add.
        /// </summary>
        /// <param name="command">
        ///     The command.
        /// </param>
        public void Add([NotNull] ICommandLineCommand command)
        {
            _commands.Add(command);
        }

        #endregion
    }
}