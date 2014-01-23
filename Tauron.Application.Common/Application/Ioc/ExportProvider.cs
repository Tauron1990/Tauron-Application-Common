// The file ExportProvider.cs is part of Tauron.Application.Common.
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
// <copyright file="ExportProvider.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The export provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Ioc
{
    /// <summary>The export provider.</summary>
    [PublicAPI]
    [ContractClass(typeof (ExportProviderContracts))]
    public abstract class ExportProvider
    {
        #region Public Events

        /// <summary>The exports changed.</summary>
        public event EventHandler<ExportChangedEventArgs> ExportsChanged;

        #endregion

        #region Public Properties

        /// <summary>Gets a value indicating whether broadcast changes.</summary>
        /// <value>The broadcast changes.</value>
        public virtual bool BroadcastChanges
        {
            get { return false; }
        }

        /// <summary>Gets the technology.</summary>
        /// <value>The technology.</value>
        public abstract string Technology { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The create exports.
        /// </summary>
        /// <param name="factory">
        ///     The factory.
        /// </param>
        /// <returns>
        ///     The <see cref="IEnumerable" />.
        /// </returns>
        public abstract IEnumerable<IExport> CreateExports(IExportFactory factory);

        #endregion

        #region Methods

        /// <summary>
        ///     The on exports changed.
        /// </summary>
        /// <param name="e">
        ///     The e.
        /// </param>
        protected virtual void OnExportsChanged(ExportChangedEventArgs e)
        {
            Contract.Requires<ArgumentNullException>(e != null, "e");

            EventHandler<ExportChangedEventArgs> handler = ExportsChanged;
            if (handler != null) handler(this, e);
        }

        #endregion
    }

    [ContractClassFor(typeof (ExportProvider))]
    internal abstract class ExportProviderContracts : ExportProvider
    {
        #region Public Properties

        /// <summary>Gets the technology.</summary>
        /// <value>The technology.</value>
        /// <exception cref="NotImplementedException"></exception>
        public override string Technology
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
        ///     The create exports.
        /// </summary>
        /// <param name="factory">
        ///     The factory.
        /// </param>
        /// <returns>
        ///     The <see cref="IEnumerable" />.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public override IEnumerable<IExport> CreateExports(IExportFactory factory)
        {
            Contract.Ensures(Contract.Result<IEnumerable<IExport>>() != null);

            throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>The export changed event args.</summary>
    [PublicAPI]
    public class ExportChangedEventArgs : EventArgs
    {
        #region Fields

        private readonly IEnumerable<ExportMetadata> added;

        private readonly IEnumerable<ExportMetadata> removed;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExportChangedEventArgs" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="ExportChangedEventArgs" /> Klasse.
        /// </summary>
        /// <param name="addedExport">
        ///     The added export.
        /// </param>
        /// <param name="removedExport">
        ///     The removed export.
        /// </param>
        public ExportChangedEventArgs(
            IEnumerable<ExportMetadata> addedExport,
            IEnumerable<ExportMetadata> removedExport)
        {
            Contract.Requires<ArgumentNullException>(addedExport != null, "addedExport");
            Contract.Requires<ArgumentNullException>(removedExport != null, "removedExport");

            added = addedExport;
            removed = removedExport;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the added.</summary>
        /// <value>The added.</value>
        public IEnumerable<ExportMetadata> Added
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<ExportMetadata>>() != null);

                return added;
            }
        }

        /// <summary>Gets or sets the removed.</summary>
        /// <value>The removed.</value>
        public IEnumerable<ExportMetadata> Removed
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<ExportMetadata>>() != null);

                return removed;
            }
        }

        #endregion
    }
}