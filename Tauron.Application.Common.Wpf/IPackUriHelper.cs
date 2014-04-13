#region

using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime.CompilerServices;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    [PublicAPI]
    [ContractClass(typeof (PackUriHelperContracts))]
    public interface IPackUriHelper
    {
        #region Public Methods and Operators

        [NotNull]
        string GetString([NotNull] string pack);

        [NotNull]
        string GetString([NotNull] string pack, [NotNull] string assembly, bool full);

        [NotNull]
        Uri GetUri([NotNull] string pack);

        [NotNull]
        Uri GetUri([NotNull] string pack, [NotNull] string assembly, bool full);

        [NotNull,MethodImpl(MethodImplOptions.NoInlining)]
        T Load<T>([NotNull] string pack) where T : class;

        [NotNull,MethodImpl(MethodImplOptions.NoInlining)]
        T Load<T>([NotNull] string pack, [NotNull] string assembly) where T : class;

        [NotNull,MethodImpl(MethodImplOptions.NoInlining)]
        Stream LoadStream([NotNull] string pack);

        [NotNull,MethodImpl(MethodImplOptions.NoInlining)]
        Stream LoadStream([NotNull] string pack, [NotNull] string assembly);

        #endregion
    }

    [ContractClassFor(typeof (IPackUriHelper))]
    internal abstract class PackUriHelperContracts : IPackUriHelper
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The get string.
        /// </summary>
        /// <param name="pack">
        ///     The pack.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public string GetString(string pack)
        {
            Contract.Requires<ArgumentNullException>(pack != null, "pack");
            Contract.Ensures(Contract.Result<string>() != null);

            throw new NotImplementedException();
        }

        /// <summary>
        ///     The get string.
        /// </summary>
        /// <param name="pack">
        ///     The pack.
        /// </param>
        /// <param name="assembly">
        ///     The assembly.
        /// </param>
        /// <param name="full">
        ///     The full.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public string GetString(string pack, string assembly, bool full)
        {
            Contract.Requires<ArgumentNullException>(pack != null, "pack");
            Contract.Requires<ArgumentNullException>(assembly != null, "assembly");

            Contract.Ensures(Contract.Result<string>() != null);

            throw new NotImplementedException();
        }

        /// <summary>
        ///     The get uri.
        /// </summary>
        /// <param name="pack">
        ///     The pack.
        /// </param>
        /// <returns>
        ///     The <see cref="Uri" />.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public Uri GetUri(string pack)
        {
            Contract.Requires<ArgumentNullException>(pack != null, "pack");

            Contract.Ensures(Contract.Result<Uri>() != null);

            throw new NotImplementedException();
        }

        /// <summary>
        ///     The get uri.
        /// </summary>
        /// <param name="pack">
        ///     The pack.
        /// </param>
        /// <param name="assembly">
        ///     The assembly.
        /// </param>
        /// <param name="full">
        ///     The full.
        /// </param>
        /// <returns>
        ///     The <see cref="Uri" />.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public Uri GetUri(string pack, string assembly, bool full)
        {
            Contract.Requires<ArgumentNullException>(pack != null, "pack");
            Contract.Requires<ArgumentNullException>(assembly != null, "assembly");

            Contract.Ensures(Contract.Result<Uri>() != null);

            throw new NotImplementedException();
        }

        /// <summary>
        ///     The load.
        /// </summary>
        /// <param name="pack">
        ///     The pack.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="T" />.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public T Load<T>(string pack) where T : class
        {
            Contract.Requires<ArgumentNullException>(pack != null, "pack");

            Contract.Ensures(Contract.Result<T>() != null);

            throw new NotImplementedException();
        }

        /// <summary>
        ///     The load.
        /// </summary>
        /// <param name="pack">
        ///     The pack.
        /// </param>
        /// <param name="assembly">
        ///     The assembly.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="T" />.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public T Load<T>(string pack, string assembly) where T : class
        {
            Contract.Requires<ArgumentNullException>(assembly != null, "assembly");
            Contract.Requires<ArgumentNullException>(pack != null, "pack");

            Contract.Ensures(Contract.Result<T>() != null);

            throw new NotImplementedException();
        }

        /// <summary>
        ///     The load stream.
        /// </summary>
        /// <param name="pack">
        ///     The pack.
        /// </param>
        /// <returns>
        ///     The <see cref="Stream" />.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public Stream LoadStream(string pack)
        {
            Contract.Requires<ArgumentNullException>(pack != null, "pack");

            Contract.Ensures(Contract.Result<Stream>() != null);

            throw new NotImplementedException();
        }

        /// <summary>
        ///     The load stream.
        /// </summary>
        /// <param name="pack">
        ///     The pack.
        /// </param>
        /// <param name="assembly">
        ///     The assembly.
        /// </param>
        /// <returns>
        ///     The <see cref="Stream" />.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public Stream LoadStream(string pack, string assembly)
        {
            Contract.Requires<ArgumentNullException>(assembly != null, "assembly");
            Contract.Requires<ArgumentNullException>(pack != null, "pack");

            Contract.Ensures(Contract.Result<Stream>() != null);

            throw new NotImplementedException();
        }

        #endregion
    }
}