#region

using System;
using System.Diagnostics.Contracts;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The ClipboardManager interface.</summary>
    [ContractClass(typeof (ClipboardManagerContracts))]
    public interface IClipboardManager
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The create viewer.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <param name="registerForClose">
        ///     The register for close.
        /// </param>
        /// <param name="performInitialization">
        ///     The perform initialization.
        /// </param>
        /// <returns>
        ///     The <see cref="ClipboardViewer" />.
        /// </returns>
        [NotNull]
        ClipboardViewer CreateViewer([NotNull] IWindow target, bool registerForClose, bool performInitialization);

        #endregion
    }

    [ContractClassFor(typeof (IClipboardManager))]
    internal abstract class ClipboardManagerContracts : IClipboardManager
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The create viewer.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <param name="registerForClose">
        ///     The register for close.
        /// </param>
        /// <param name="performInitialization">
        ///     The perform initialization.
        /// </param>
        /// <returns>
        ///     The <see cref="ClipboardViewer" />.
        /// </returns>
        public ClipboardViewer CreateViewer(IWindow target, bool registerForClose, bool performInitialization)
        {
            Contract.Requires<ArgumentNullException>(target != null, "target");
            Contract.Ensures(Contract.Result<ClipboardViewer>() != null);

            return null;
        }

        #endregion
    }
}