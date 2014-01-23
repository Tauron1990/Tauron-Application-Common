// The file IExportFactory.cs is part of Tauron.Application.Common.
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
// <copyright file="IExportFactory.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The ExportFactory interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.Contracts;
using Tauron.Application.Ioc.Components;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Exports
{
    /// <summary>The ExportFactory interface.</summary>
    [PublicAPI]
    [ContractClass(typeof (ExportFactoryContracts))]
    public interface IExportFactory : IInitializeable
    {
        #region Public Properties

        /// <summary>Gets the technology name.</summary>
        /// <value>The technology name.</value>
        string TechnologyName { get; }

        #endregion
    }

    [ContractClassFor(typeof (IExportFactory))]
    internal abstract class ExportFactoryContracts : IExportFactory
    {
        #region Public Properties

        /// <summary>Gets the technology name.</summary>
        /// <value>The technology name.</value>
        /// <exception cref="NotImplementedException"></exception>
        public string TechnologyName
        {
            get
            {
                Contract.Ensures(Contract.Result<string>() != null);
                throw new NotImplementedException();
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The initialize.
        /// </summary>
        /// <param name="components">
        ///     The components.
        /// </param>
        public void Initialize(ComponentRegistry components)
        {
        }

        #endregion
    }
}