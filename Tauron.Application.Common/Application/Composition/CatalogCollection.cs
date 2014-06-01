#region

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

        private Collection<XamlCatalog> _catalogs;

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
        [NotNull,SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public Collection<XamlCatalog> Catalogs
        {
            get
            {
                Contract.Ensures(Contract.Result<Collection<XamlCatalog>>() != null);

                return _catalogs;
            }

            set
            {
                Contract.Requires<ArgumentNullException>(value != null, "value");

                _catalogs = value;
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
        public void FillCatalag([NotNull] ExportResolver container)
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