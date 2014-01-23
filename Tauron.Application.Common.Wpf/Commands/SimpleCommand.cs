// The file SimpleCommand.cs is part of Tauron.Application.Common.Wpf.
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
// <copyright file="SimpleCommand.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The simple command.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Windows.Input;

#endregion

namespace Tauron.Application.Commands
{
    /// <summary>The simple command.</summary>
    public class SimpleCommand : ICommand
    {
        #region Fields

        private readonly Func<object, bool> canExecute;

        private readonly Action<object> execute;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SimpleCommand" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="SimpleCommand" /> Klasse.
        /// </summary>
        /// <param name="canExecute">
        ///     The can execute.
        /// </param>
        /// <param name="execute">
        ///     The execute.
        /// </param>
        public SimpleCommand(Func<object, bool> canExecute, Action<object> execute)
        {
            this.canExecute = canExecute;
            this.execute = execute;
        }

        #endregion

        #region Public Events

        /// <summary>The can execute changed.</summary>
        public event EventHandler CanExecuteChanged
        {
            add { if (canExecute != null) CommandManager.RequerySuggested += value; }

            remove { if (canExecute != null) CommandManager.RequerySuggested -= value; }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The can execute.
        /// </summary>
        /// <param name="parameter">
        ///     The parameter.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute(parameter);
        }

        /// <summary>
        ///     The execute.
        /// </summary>
        /// <param name="parameter">
        ///     The parameter.
        /// </param>
        public void Execute(object parameter)
        {
            if (execute != null) execute(parameter);
        }

        #endregion
    }
}