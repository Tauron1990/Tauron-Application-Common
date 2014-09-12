#region

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