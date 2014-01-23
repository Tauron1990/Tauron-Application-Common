// The file IUiControllerFactory.cs is part of Tauron.Application.Common.
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
// <copyright file="IUiControllerFactory.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The UIControllerFactory interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.Contracts;

#endregion

namespace Tauron.Application
{
    /// <summary>The UIControllerFactory interface.</summary>
    [ContractClass(typeof (UIControllerFactoryContracts))]
    public interface IUIControllerFactory
    {
        #region Public Methods and Operators

        /// <summary>The create controller.</summary>
        /// <returns>
        ///     The <see cref="IUIController" />.
        /// </returns>
        IUIController CreateController();

        /// <summary>The set synchronization context.</summary>
        void SetSynchronizationContext();

        #endregion
    }

    [ContractClassFor(typeof (IUIControllerFactory))]
    internal abstract class UIControllerFactoryContracts : IUIControllerFactory
    {
        #region Public Methods and Operators

        /// <summary>The create controller.</summary>
        /// <returns>
        ///     The <see cref="IUIController" />.
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        public IUIController CreateController()
        {
            Contract.Ensures(Contract.Result<IUIController>() != null);
            throw new NotImplementedException();
        }

        /// <summary>The set synchronization context.</summary>
        /// <exception cref="NotImplementedException"></exception>
        public void SetSynchronizationContext()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}