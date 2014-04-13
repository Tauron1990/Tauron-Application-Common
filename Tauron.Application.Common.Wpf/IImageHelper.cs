#region

using System;
using System.Diagnostics.Contracts;
using System.Windows.Media;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The ImageHelper interface.</summary>
    [ContractClass(typeof (ImageHelperContracts))]
    public interface IImageHelper
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The convert.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <param name="assembly">
        ///     The assembly.
        /// </param>
        /// <returns>
        ///     The <see cref="ImageSource" />.
        /// </returns>
        [CanBeNull]
        ImageSource Convert([NotNull] Uri target, [NotNull] string assembly);

        /// <summary>
        ///     The convert.
        /// </summary>
        /// <param name="uri">
        ///     The uri.
        /// </param>
        /// <param name="assembly">
        ///     The assembly.
        /// </param>
        /// <returns>
        ///     The <see cref="ImageSource" />.
        /// </returns>
        [CanBeNull]
        ImageSource Convert([NotNull] string uri, [NotNull] string assembly);

        #endregion
    }

    [ContractClassFor(typeof (IImageHelper))]
    internal abstract class ImageHelperContracts : IImageHelper
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The convert.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <param name="assembly">
        ///     The assembly.
        /// </param>
        /// <returns>
        ///     The <see cref="ImageSource" />.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public ImageSource Convert(Uri target, string assembly)
        {
            Contract.Requires<ArgumentNullException>(target != null, "target");

            throw new NotImplementedException();
        }

        /// <summary>
        ///     The convert.
        /// </summary>
        /// <param name="uri">
        ///     The uri.
        /// </param>
        /// <param name="assembly">
        ///     The assembly.
        /// </param>
        /// <returns>
        ///     The <see cref="ImageSource" />.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public ImageSource Convert(string uri, string assembly)
        {
            Contract.Requires<ArgumentNullException>(uri != null, "uri");

            throw new NotImplementedException();
        }

        #endregion
    }
}