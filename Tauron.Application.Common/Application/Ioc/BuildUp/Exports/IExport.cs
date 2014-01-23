// The file IExport.cs is part of Tauron.Application.Common.
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
// <copyright file="IExport.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The Export interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Exports
{
    /// <summary>The Export interface.</summary>
    [PublicAPI]
    [ContractClass(typeof (ExportContracts))]
    public interface IExport : IEquatable<IExport>
    {
        #region Public Properties

        /// <summary>Gets the export metadata.</summary>
        /// <value>The export metadata.</value>
        IEnumerable<ExportMetadata> ExportMetadata { get; }

        /// <summary>Gets the exports.</summary>
        /// <value>The exports.</value>
        IEnumerable<Type> Exports { get; }

        /// <summary>Gets the external info.</summary>
        /// <value>The external info.</value>
        ExternalExportInfo ExternalInfo { get; }

        /// <summary>Gets or sets the globalmetadata.</summary>
        /// <value>The globalmetadata.</value>
        Dictionary<string, object> Globalmetadata { get; }

        /// <summary>Gets the implement type.</summary>
        /// <value>The implement type.</value>
        Type ImplementType { get; }

        /// <summary>Gets the import metadata.</summary>
        /// <value>The import metadata.</value>
        IEnumerable<ImportMetadata> ImportMetadata { get; }

        /// <summary>Gets a value indicating whether share lifetime.</summary>
        /// <value>The share lifetime.</value>
        bool ShareLifetime { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The get export metadata.
        /// </summary>
        /// <param name="contractName">
        ///     The contract name.
        /// </param>
        /// <returns>
        ///     The <see cref="ExportMetadata" />.
        /// </returns>
        ExportMetadata GetNamedExportMetadata(string contractName);

        /// <summary>
        ///     The select contract name.
        /// </summary>
        /// <param name="contractName">
        ///     The contract name.
        /// </param>
        /// <returns>
        ///     The <see cref="IEnumerable" />.
        /// </returns>
        IEnumerable<ExportMetadata> SelectContractName(string contractName);

        #endregion
    }

    [ContractClassFor(typeof (IExport))]
    internal abstract class ExportContracts : IExport
    {
        #region Public Properties

        /// <summary>Gets the export metadata.</summary>
        /// <value>The export metadata.</value>
        /// <exception cref="NotImplementedException"></exception>
        public IEnumerable<ExportMetadata> ExportMetadata
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<ExportMetadata>>() != null);

                throw new NotImplementedException();
            }
        }

        /// <summary>Gets the exports.</summary>
        /// <value>The exports.</value>
        /// <exception cref="NotImplementedException"></exception>
        public IEnumerable<Type> Exports
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<Type>>() != null);

                throw new NotImplementedException();
            }
        }

        /// <summary>Gets the external info.</summary>
        /// <value>The external info.</value>
        /// <exception cref="NotImplementedException"></exception>
        public ExternalExportInfo ExternalInfo
        {
            get
            {
                Contract.Ensures(Contract.Result<ExternalExportInfo>() != null);

                throw new NotImplementedException();
            }
        }

        /// <summary>Gets the globalmetadata.</summary>
        /// <value>The globalmetadata.</value>
        /// <exception cref="NotImplementedException"></exception>
        public Dictionary<string, object> Globalmetadata
        {
            get
            {
                Contract.Ensures(Contract.Result<Dictionary<string, object>>() != null);

                throw new NotImplementedException();
            }
        }

        /// <summary>Gets the implement type.</summary>
        /// <value>The implement type.</value>
        /// <exception cref="NotImplementedException"></exception>
        public Type ImplementType
        {
            get
            {
                Contract.Ensures(Contract.Result<Type>() != null);

                throw new NotImplementedException();
            }
        }

        /// <summary>Gets the import metadata.</summary>
        /// <value>The import metadata.</value>
        /// <exception cref="NotImplementedException"></exception>
        public IEnumerable<ImportMetadata> ImportMetadata
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<ImportMetadata>>() != null);

                throw new NotImplementedException();
            }
        }

        /// <summary>Gets a value indicating whether share lifetime.</summary>
        /// <value>The share lifetime.</value>
        /// <exception cref="NotImplementedException"></exception>
        public bool ShareLifetime
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The equals.
        /// </summary>
        /// <param name="other">
        ///     The other.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public bool Equals(IExport other)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     The get named export metadata.
        /// </summary>
        /// <param name="contractName">
        ///     The contract name.
        /// </param>
        /// <returns>
        ///     The <see cref="ExportMetadata" />.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public ExportMetadata GetNamedExportMetadata(string contractName)
        {
            Contract.Ensures(Contract.Result<ExportMetadata>() != null);

            throw new NotImplementedException();
        }

        /// <summary>
        ///     The select contract name.
        /// </summary>
        /// <param name="contractName">
        ///     The contract name.
        /// </param>
        /// <returns>
        ///     The <see cref="IEnumerable" />.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public IEnumerable<ExportMetadata> SelectContractName(string contractName)
        {
            Contract.Ensures(Contract.Result<IEnumerable<ExportMetadata>>() != null);

            throw new NotImplementedException();
        }

        #endregion
    }
}