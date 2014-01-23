// The file IShellFactory.cs is part of Tauron.Application.Common.
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
// <copyright file="IShellFactory.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The ShellFactory interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.Contracts;

#endregion

namespace Tauron.Application
{
    /// <summary>The ShellFactory interface.</summary>
    [ContractClass(typeof (ShellFactoryContracts))]
    public interface IShellFactory
    {
        #region Public Methods and Operators

        /// <summary>The create view.</summary>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        object CreateView();

        #endregion
    }

    [ContractClassFor(typeof (IShellFactory))]
    internal abstract class ShellFactoryContracts : IShellFactory
    {
        #region Public Methods and Operators

        /// <summary>The create view.</summary>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        public object CreateView()
        {
            Contract.Ensures(Contract.Result<object>() != null);
            return null;
        }

        #endregion
    }
}