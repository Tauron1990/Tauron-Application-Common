#region

using System;
using System.Diagnostics.Contracts;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The UIControllerFactory interface.</summary>
    [ContractClass(typeof (UIControllerFactoryContracts))]
    public interface IUIControllerFactory
    {
        #region Public Methods and Operators

        /// <summary>The create controller.</summary>
        /// <returns>
        ///     The <see cref="IUIController" />.
        /// </returns>
        [NotNull]
        IUIController CreateController();

        /// <summary>The set synchronization context.</summary>
        void SetSynchronizationContext();

        #endregion
    }

    [ContractClassFor(typeof (IUIControllerFactory))]
    internal abstract class UIControllerFactoryContracts : IUIControllerFactory
    {
        #region Public Methods and Operators

        /// <summary>The create controller.</summary>
        /// <returns>
        ///     The <see cref="IUIController" />.
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        public IUIController CreateController()
        {
            Contract.Ensures(Contract.Result<IUIController>() != null);
            throw new NotImplementedException();
        }

        /// <summary>The set synchronization context.</summary>
        /// <exception cref="NotImplementedException"></exception>
        public void SetSynchronizationContext()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}