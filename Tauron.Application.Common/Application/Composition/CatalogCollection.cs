// The file CatalogCollection.cs is part of Tauron.Application.Common.
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
// <copyright file="CatalogCollection.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The catalog collection.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Windows.Markup;
using Tauron.Application.Ioc;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Composition
{
    /// <summary>The catalog collection.</summary>
    [ContentProperty("Catalogs")]
    [DefaultProperty("Catalogs")]
    [PublicAPI]
    public sealed class CatalogCollection
    {
        #region Fields

        private Collection<XamlCatalog> mcatalogs;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="CatalogCollection" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="CatalogCollection" /> Klasse.
        ///     Initializes a new instance of the <see cref="CatalogCollection" /> class.
        /// </summary>
        public CatalogCollection()
        {
            Catalogs = new Collection<XamlCatalog>();
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the catalogs.</summary>
        /// <value>The catalogs.</value>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public Collection<XamlCatalog> Catalogs
        {
            get
            {
                Contract.Ensures(Contract.Result<Collection<XamlCatalog>>() != null);

                return mcatalogs;
            }

            set
            {
                Contract.Requires<ArgumentNullException>(value != null, "value");

                mcatalogs = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The fill catalag.
        /// </summary>
        /// <param name="container">
        ///     The container.
        /// </param>
        public void FillCatalag(ExportResolver container)
        {
            Contract.Requires<ArgumentNullException>(container != null, "container");

            foreach (XamlCatalog xamlCatalog in Catalogs) xamlCatalog.FillContainer(container);
        }

        #endregion

        #region Methods

        /// <summary>The object invariant.</summary>
        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(Catalogs != null, "Catalog Collection: Catalog Null");
        }

        #endregion
    }
}