// The file ExportRegistry.cs is part of Tauron.Application.Common.
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
// <copyright file="ExportRegistry.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The export registry.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Ioc.Components
{
    /// <summary>The export registry.</summary>
    [PublicAPI]
    public sealed class ExportRegistry
    {
        #region Fields

        /// <summary>The _registrations.</summary>
        private readonly GroupDictionary<Type, IExport> _registrations = new GroupDictionary<Type, IExport>(true);

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The find all.
        /// </summary>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <param name="contractName">
        ///     The contract name.
        /// </param>
        /// <param name="errorTracer"></param>
        /// <returns>
        ///     The <see cref="IEnumerable" />.
        /// </returns>
        public IEnumerable<ExportMetadata> FindAll(Type type, string contractName, ErrorTracer errorTracer)
        {
            Contract.Requires<ArgumentNullException>(type != null, "type");
            Contract.Ensures(Contract.Result<IEnumerable<ExportMetadata>>() != null);

            try
            {
                lock (this)
                {
                    errorTracer.Phase = "Getting Exports by Type (" + type + ")";
                    IEnumerable<IExport> regs = _registrations[type];

                    errorTracer.Phase = "Filtering Exports by Contract Name (" + contractName + ")";
                    return regs.SelectMany(ex => ex.SelectContractName(contractName)).Where(exp => exp != null);
                }
            }
            catch (Exception e)
            {
                errorTracer.Exception = e;
                errorTracer.Exceptional = true;
                errorTracer.Export = ErrorTracer.FormatExport(type, contractName);

                return Enumerable.Empty<ExportMetadata>();
            }
        }

        /// <summary>
        ///     The find optional.
        /// </summary>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <param name="contractName">
        ///     The contract name.
        /// </param>
        /// <param name="errorTracer"></param>
        /// <returns>
        ///     The <see cref="ExportMetadata" />.
        /// </returns>
        public ExportMetadata FindOptional(Type type, string contractName, ErrorTracer errorTracer)
        {
            Contract.Requires<ArgumentNullException>(type != null, "type");

            lock (this)
            {
                ExportMetadata[] arr = FindAll(type, contractName, errorTracer).ToArray();

                if (errorTracer.Exceptional)
                    return null;

                errorTracer.Phase = "Getting Single Instance";

                if (arr.Length > 1)
                {
                    errorTracer.Exceptional = true;
                    errorTracer.Exception = new InvalidOperationException("More then One Export Found");
                    errorTracer.Export = ErrorTracer.FormatExport(type, contractName);
                }

                return arr.Length == 0 ? null : arr[0];
            }
        }

        /// <summary>
        ///     The find single.
        /// </summary>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <param name="contractName">
        ///     The contract name.
        /// </param>
        /// <param name="errorTracer"></param>
        /// <returns>
        ///     The <see cref="ExportMetadata" />.
        /// </returns>
        public ExportMetadata FindSingle(Type type, string contractName, ErrorTracer errorTracer)
        {
            Contract.Requires<ArgumentNullException>(type != null, "type");
            Contract.Requires<ArgumentNullException>(errorTracer != null, "errorTracer");

            var temp = FindOptional(type, contractName, errorTracer);
            if (errorTracer.Exceptional) return null;
            if (temp == null)
            {
                errorTracer.Exceptional = true;
                errorTracer.Exception = new InvalidOperationException("No Export Found");
                errorTracer.Export = ErrorTracer.FormatExport(type, contractName);
            }

            return temp;
        }

        /// <summary>
        ///     The register.
        /// </summary>
        /// <param name="export">
        ///     The export.
        /// </param>
        public void Register(IExport export)
        {
            Contract.Requires<ArgumentNullException>(export != null, "export");

            lock (this)
            {
                foreach (Type type in export.Exports) _registrations.Add(type, export);
            }
        }

        /// <summary>
        ///     The remove.
        /// </summary>
        /// <param name="export">
        ///     The export.
        /// </param>
        public void Remove(IExport export)
        {
            Contract.Requires<ArgumentNullException>(export != null, "export");

            lock (this)
            {
                _registrations.RemoveValue(export);
            }
        }

        #endregion
    }
}