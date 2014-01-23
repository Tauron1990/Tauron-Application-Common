// The file INotifyPropertyChangedMethod.cs is part of Tauron.Application.Common.
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
// <copyright file="INotifyPropertyChangedMethod.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The NotifyPropertyChangedMethod interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Aop.Model
{
    /// <summary>The NotifyPropertyChangedMethod interface.</summary>
    [PublicAPI]
    [ContractClass(typeof (INotifyPropertyChangedMethodContracts))]
    public interface INotifyPropertyChangedMethod : INotifyPropertyChanged
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The on property changed.
        /// </summary>
        /// <param name="eventArgs">
        ///     The event args.
        /// </param>
        void OnPropertyChanged(string eventArgs);

        #endregion
    }

    [ContractClassFor(typeof (INotifyPropertyChangedMethod))]
    internal abstract class INotifyPropertyChangedMethodContracts : INotifyPropertyChangedMethod
    {
        #region Public Events

        /// <summary>The property changed.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The on property changed.
        /// </summary>
        /// <param name="eventArgs">
        ///     The event args.
        /// </param>
        public void OnPropertyChanged(string eventArgs)
        {
            Contract.Requires<ArgumentNullException>(eventArgs != null, "eventArgs");
        }

        #endregion
    }
}