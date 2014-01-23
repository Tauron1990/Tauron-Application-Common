// The file ICache.cs is part of Tauron.Application.Common.
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
//  along with Tauron.Application.Common If not, see <http://www.gnu.org/licenses/>.

#region

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICache.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The Cache interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.Contracts;
using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.Application.Ioc.LifeTime;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Strategy
{
    /// <summary>The Cache interface.</summary>
    [ContractClass(typeof (CacheContracts))]
    public interface ICache
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The add.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <param name="metadata">
        ///     The metadata.
        /// </param>
        /// <param name="shareLifetime">
        ///     The share lifetime.
        /// </param>
        void Add(ILifetimeContext context, ExportMetadata metadata, bool shareLifetime);

        /// <summary>
        ///     The get.
        /// </summary>
        /// <param name="metadata">
        ///     The metadata.
        /// </param>
        /// <returns>
        ///     The <see cref="ILifetimeContext" />.
        /// </returns>
        ILifetimeContext GetContext(ExportMetadata metadata);

        #endregion
    }

    [ContractClassFor(typeof (ICache))]
    internal abstract class CacheContracts : ICache
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The add.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <param name="metadata">
        ///     The metadata.
        /// </param>
        /// <param name="shareLifetime">
        ///     The share lifetime.
        /// </param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public void Add(ILifetimeContext context, ExportMetadata metadata, bool shareLifetime)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
            Contract.Requires<ArgumentNullException>(metadata != null, "metadata");

            throw new NotImplementedException();
        }

        /// <summary>
        ///     The get context.
        /// </summary>
        /// <param name="metadata">
        ///     The metadata.
        /// </param>
        /// <returns>
        ///     The <see cref="ILifetimeContext" />.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public ILifetimeContext GetContext(ExportMetadata metadata)
        {
            Contract.Requires<ArgumentNullException>(metadata != null, "metadata");

            throw new NotImplementedException();
        }

        #endregion
    }
}