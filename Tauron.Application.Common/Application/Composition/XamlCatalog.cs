#region

using System;
using System.Diagnostics.Contracts;
using Tauron.Application.Ioc;
using Tauron.JetBrains.Annotations;

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
        protected internal abstract void FillContainer([NotNull] ExportResolver container);

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