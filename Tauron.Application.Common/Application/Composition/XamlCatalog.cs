// The file XamlCatalog.cs is part of Tauron.Application.Common.
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
// <copyright file="XamlCatalog.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The xaml catalog.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.Contracts;
using Tauron.Application.Ioc;

#endregion

namespace Tauron.Application.Composition
{
    /// <summary>The xaml catalog.</summary>
    [ContractClass(typeof (XamlCatalogContracts))]
    public abstract class XamlCatalog
    {
        #region Methods

        /// <summary>
        ///     The fill container.
        /// </summary>
        /// <param name="container">
        ///     The container.
        /// </param>
        protected internal abstract void FillContainer(ExportResolver container);

        #endregion
    }

    /// <summary>The xaml catalog contracts.</summary>
    [ContractClassFor(typeof (XamlCatalog))]
    internal abstract class XamlCatalogContracts : XamlCatalog
    {
        #region Methods

        /// <summary>
        ///     The fill container.
        /// </summary>
        /// <param name="container">
        ///     The container.
        /// </param>
        protected internal override void FillContainer(ExportResolver container)
        {
            Contract.Requires<ArgumentNullException>(container != null, "container");
        }

        #endregion
    }
}