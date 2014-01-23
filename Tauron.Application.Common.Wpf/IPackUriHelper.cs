// The file IPackUriHelper.cs is part of Tauron.Application.Common.Wpf.
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
//  along with Tauron.Application.Common.Wpf If not, see <http://www.gnu.org/licenses/>.

#region

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackUriHelper.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The PackUriHelper interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime.CompilerServices;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The PackUriHelper interface.</summary>
    [PublicAPI]
    [ContractClass(typeof (PackUriHelperContracts))]
    public interface IPackUriHelper
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
        [NotNull]
        string GetString([NotNull] string pack);

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
        [NotNull]
        string GetString([NotNull] string pack, [NotNull] string assembly, bool full);

        /// <summary>
        ///     The get uri.
        /// </summary>
        /// <param name="pack">
        ///     The pack.
        /// </param>
        /// <returns>
        ///     The <see cref="Uri" />.
        /// </returns>
        [NotNull]
        Uri GetUri([NotNull] string pack);

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
        [NotNull]
        Uri GetUri([NotNull] string pack, [NotNull] string assembly, bool full);

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
        [NotNull,MethodImpl(MethodImplOptions.NoInlining)]
        T Load<T>([NotNull] string pack) where T : class;

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
        [NotNull,MethodImpl(MethodImplOptions.NoInlining)]
        T Load<T>([NotNull] string pack, [NotNull] string assembly) where T : class;

        /// <summary>
        ///     The load stream.
        /// </summary>
        /// <param name="pack">
        ///     The pack.
        /// </param>
        /// <returns>
        ///     The <see cref="Stream" />.
        /// </returns>
        [NotNull,MethodImpl(MethodImplOptions.NoInlining)]
        Stream LoadStream([NotNull] string pack);

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