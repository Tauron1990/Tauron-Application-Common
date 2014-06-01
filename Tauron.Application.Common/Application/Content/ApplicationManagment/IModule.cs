#region

using System.Diagnostics.Contracts;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The Module interface.</summary>
    [ContractClass(typeof (IModuleContracts)), PublicAPI]
    public interface IModule
    {
        int Order { get; }

        #region Public Methods and Operators

        /// <summary>
        ///     The initialize.
        /// </summary>
        /// <param name="application">
        ///     The application.
        /// </param>
        void Initialize([NotNull] CommonApplication application);

        #endregion
    }

    /// <summary>The i module contracts.</summary>
    [ContractClassFor(typeof (IModule))]
    internal abstract class IModuleContracts : IModule
    {
        #region Public Methods and Operators

        public int Order { get; private set; }

        /// <summary>
        ///     The initialize.
        /// </summary>
        /// <param name="application">
        ///     The application.
        /// </param>
        public void Initialize(CommonApplication application)
        {
            Contract.Requires(application != null);
        }

        #endregion
    }
}