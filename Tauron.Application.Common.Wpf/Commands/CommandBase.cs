// The file CommandBase.cs is part of Tauron.Application.Common.Wpf.
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
// <copyright file="CommandBase.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   Die Basis Klasse für alle Commands.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Commands
{
    /// <summary>
    ///     Die Basis Klasse für alle Commands.
    /// </summary>
    public abstract class CommandBase : ICommand
    {
        #region Public Events

        /// <summary>
        ///     Tritt ein, wenn Änderungen auftreten, die sich auf die Ausführung des Befehls auswirken.
        /// </summary>
        public virtual event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }

            remove { CommandManager.RequerySuggested -= value; }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Definiert die Methode, mit der ermittelt wird, ob der Befehl im aktuellen Zustand ausgeführt werden kann.
        /// </summary>
        /// <returns>
        ///     true, wenn der Befehl ausgeführt werden kann, andernfalls false.
        /// </returns>
        /// <param name="parameter">
        ///     Daten, die vom Befehl verwendet werden.Wenn der Befehl keine Datenübergabe erfordert, kann das Objekt auf null
        ///     festgelegt werden.
        /// </param>
        public virtual bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        ///     Definiert die Methode, die aufgerufen werden soll, wenn der Befehl aufgerufen wird.
        /// </summary>
        /// <param name="parameter">
        ///     Daten, die vom Befehl verwendet werden.Wenn der Befehl keine Datenübergabe erfordert, kann das Objekt auf null
        ///     festgelegt werden.
        /// </param>
        public abstract void Execute(object parameter);

        /// <summary>
        ///     Ruft das Event CanExecuteChanged auf.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        [UsedImplicitly]
        // ReSharper disable VirtualMemberNeverOverriden.Global
        public virtual void RaiseCanExecuteChanged()
        {
            // ReSharper restore VirtualMemberNeverOverriden.Global
            CommandManager.InvalidateRequerySuggested();
        }

        #endregion
    }
}