// The file IContainerExtension.cs is part of Tauron.Application.Common.
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
// <copyright file="IContainerExtension.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The ContainerExtension interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.Contracts;
using Tauron.Application.Ioc.Components;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Ioc
{
    /// <summary>The ContainerExtension interface.</summary>
    [PublicAPI]
    [ContractClass(typeof (ContainerExtensionContracts))]
    public interface IContainerExtension
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The initialize.
        /// </summary>
        /// <param name="components">
        ///     The components.
        /// </param>
        void Initialize([NotNull] ComponentRegistry components);

        #endregion
    }

    [ContractClassFor(typeof (IContainerExtension))]
    internal abstract class ContainerExtensionContracts : IContainerExtension
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The initialize.
        /// </summary>
        /// <param name="components">
        ///     The components.
        /// </param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public void Initialize(ComponentRegistry components)
        {
            Contract.Requires<ArgumentNullException>(components != null, "components");

            throw new NotImplementedException();
        }

        #endregion
    }
}