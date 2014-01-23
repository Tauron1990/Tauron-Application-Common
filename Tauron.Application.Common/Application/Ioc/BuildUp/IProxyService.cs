// The file IProxyService.cs is part of Tauron.Application.Common.
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
// <copyright file="IProxyService.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The ProxyService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.Contracts;
using Castle.DynamicProxy;
using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Ioc.BuildUp
{
    /// <summary>The ProxyService interface.</summary>
    [ContractClass(typeof (ProxyServiceContracts)), PublicAPI]
    public interface IProxyService
    {
        #region Public Properties

        /// <summary>Gets the generator.</summary>
        /// <value>The generator.</value>
        [NotNull]
        ProxyGenerator Generate([NotNull] ExportMetadata metadata, [NotNull] ImportMetadata[] imports,  out IImportInterceptor interceptor);

        #endregion

        [NotNull]
        ProxyGenerator GenericGenerator { get; }
    }

    [ContractClassFor(typeof (IProxyService))]
    internal abstract class ProxyServiceContracts : IProxyService
    {
        public ProxyGenerator Generate(ExportMetadata metadata, ImportMetadata[] imports, out IImportInterceptor interceptor)
        {
            Contract.Requires<ArgumentNullException>(metadata != null, "metadata");
            Contract.Requires<ArgumentNullException>(imports != null, "imports");
            Contract.Ensures(Contract.Result<ProxyGenerator>() != null);

            interceptor = null;

            return null;
        }

        public ProxyGenerator GenericGenerator
        {
            get
            {
                Contract.Ensures(Contract.Result<ProxyGenerator>() != null);

                return null;
            }
        }
    }
}