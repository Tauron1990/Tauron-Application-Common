// The file EventCommand.cs is part of Tauron.Application.Common.Wpf.
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
// <copyright file="EventCommand.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The event command.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Commands
{
    /// <summary>The event command.</summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public sealed class EventCommand : CommandBase
    {
        #region Public Events

        /// <summary>The can execute event.</summary>
        [SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Func<object, bool> CanExecuteEvent;

        /// <summary>The execute event.</summary>
        [SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Action<object> ExecuteEvent;

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
        public override bool CanExecute(object parameter)
        {
            return OnCanExecute(parameter);
        }

        /// <summary>
        ///     The execute.
        /// </summary>
        /// <param name="parameter">
        ///     The parameter.
        /// </param>
        public override void Execute(object parameter)
        {
            OnExecute(parameter);
        }

        #endregion

        #region Methods

        private bool OnCanExecute(object parameter)
        {
            Func<object, bool> handler = CanExecuteEvent;
            if (handler != null) return handler(parameter);

            return true;
        }

        private void OnExecute(object parameter)
        {
            Action<object> handler = ExecuteEvent;
            if (handler != null) handler(parameter);
        }

        #endregion
    }
}