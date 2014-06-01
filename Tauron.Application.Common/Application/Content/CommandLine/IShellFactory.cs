#region

using System.Diagnostics.Contracts;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The ShellFactory interface.</summary>
    [ContractClass(typeof (ShellFactoryContracts))]
    public interface IShellFactory
    {
        #region Public Methods and Operators

        /// <summary>The create view.</summary>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        [NotNull]
        object CreateView();

        #endregion
    }

    [ContractClassFor(typeof (IShellFactory))]
    internal abstract class ShellFactoryContracts : IShellFactory
    {
        #region Public Methods and Operators

        /// <summary>The create view.</summary>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        public object CreateView()
        {
            Contract.Ensures(Contract.Result<object>() != null);
            return null;
        }

        #endregion
    }
}