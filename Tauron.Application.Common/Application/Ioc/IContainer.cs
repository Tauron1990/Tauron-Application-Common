// The file IContainer.cs is part of Tauron.Application.Common.
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
// <copyright file="IContainer.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The Container interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Ioc
{
    /// <summary>The Container interface.</summary>
    [PublicAPI]
    [ContractClass(typeof (ContainerContracts))]
    public interface IContainer : IDisposable
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The build up.
        /// </summary>
        /// <param name="data">
        ///     The data.
        /// </param>
        /// <param name="errorTracer"></param>
        /// <param name="parameters"></param>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        object BuildUp(ExportMetadata data, ErrorTracer errorTracer, params BuildParameter[] parameters);

        /// <summary>
        ///     The build up.
        /// </summary>
        /// <param name="toBuild">
        ///     The object.
        /// </param>
        /// <param name="errorTracer"></param>
        /// <param name="parameters"></param>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        object BuildUp(object toBuild, ErrorTracer errorTracer, params BuildParameter[] parameters);

        /// <summary>
        ///     The build up.
        /// </summary>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <param name="buildParameters"></param>
        /// <param name="constructorArguments">
        ///     The constructor arguments.
        /// </param>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        object BuildUp(Type type,  ErrorTracer errorTracer, BuildParameter[] buildParameters, params object[] constructorArguments);

        /// <summary>
        ///     The find export.
        /// </summary>
        /// <param name="interfaceType">
        ///     The interface.
        /// </param>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <param name="isOptional">
        ///     The optional.
        /// </param>
        /// <returns>
        ///     The <see cref="ExportMetadata" />.
        /// </returns>
        ExportMetadata FindExport(Type interfaceType, string name, ErrorTracer errorTracer, bool isOptional);

        /// <summary>
        ///     The find export.
        /// </summary>
        /// <param name="interfaceType">
        ///     The interface.
        /// </param>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <returns>
        ///     The <see cref="ExportMetadata" />.
        /// </returns>
        ExportMetadata FindExport(Type interfaceType, string name, ErrorTracer errorTracer);

        /// <summary>
        ///     The find exports.
        /// </summary>
        /// <param name="interfaceType">
        ///     The interface.
        /// </param>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <returns>
        ///     The <see cref="IEnumerable" />.
        /// </returns>
        [NotNull]
        IEnumerable<ExportMetadata> FindExports([NotNull] Type interfaceType, [NotNull] string name, [NotNull] ErrorTracer errorTracer);

        [NotNull]
        ExportMetadata FindExport([NotNull] Type interfaceType, [NotNull] string name, [NotNull] ErrorTracer errorTracer, bool isOptional, int level);

        /// <summary>
        ///     The find exports.
        /// </summary>
        /// <param name="interfaceType">
        ///     The interface.
        /// </param>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <returns>
        ///     The <see cref="IEnumerable" />.
        /// </returns>
        [NotNull]
        IEnumerable<ExportMetadata> FindExports([NotNull] Type interfaceType, [NotNull] string name, [NotNull] ErrorTracer errorTracer, int level);

        /// <summary>
        ///     The register.
        /// </summary>
        /// <param name="exportType">
        ///     The export.
        /// </param>
        /// <param name="level"></param>
        void Register(IExport exportType, int level);

        /// <summary>
        ///     The register.
        /// </summary>
        /// <param name="exportResolver">
        ///     The export.
        /// </param>
        void Register(ExportResolver exportResolver);

        /// <summary>
        ///     The register.
        /// </summary>
        /// <param name="extension">
        ///     The extension.
        /// </param>
        void Register(IContainerExtension extension);

        #endregion
    }

    [ContractClassFor(typeof (IContainer))]
    internal abstract class ContainerContracts : IContainer
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The build up.
        /// </summary>
        /// <param name="data">
        ///     The data.
        /// </param>
        /// <param name="errorTracer"></param>
        /// <param name="parameters"></param>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public object BuildUp(ExportMetadata data, ErrorTracer errorTracer, params BuildParameter[] parameters)
        {
            Contract.Requires<ArgumentNullException>(data != null, "data");
            Contract.Requires<ArgumentNullException>(errorTracer != null, "errorTracer");

            throw new NotImplementedException();
        }

        /// <summary>
        ///     The build up.
        /// </summary>
        /// <param name="toBuild">
        ///     The to build.
        /// </param>
        /// <param name="errorTracer"></param>
        /// <param name="parameters"></param>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public object BuildUp(object toBuild, ErrorTracer errorTracer, params BuildParameter[] parameters)
        {
            Contract.Requires<ArgumentNullException>(toBuild != null, "toBuild");
            Contract.Requires<ArgumentNullException>(errorTracer != null, "errorTracer");

            throw new NotImplementedException();
        }

        /// <summary>
        ///     The build up.
        /// </summary>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <param name="constructorArguments">
        ///     The constructor arguments.
        /// </param>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public object BuildUp(Type type, ErrorTracer errorTracer, BuildParameter[] buildParameters, params object[] constructorArguments)
        {
            Contract.Requires<ArgumentNullException>(type != null, "type");
            Contract.Requires<ArgumentNullException>(errorTracer != null, "errorTracer");

            throw new NotImplementedException();
        }

        /// <summary>The dispose.</summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     The find export.
        /// </summary>
        /// <param name="interfaceType">
        ///     The interface type.
        /// </param>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <param name="isOptional">
        ///     The is optional.
        /// </param>
        /// <returns>
        ///     The <see cref="ExportMetadata" />.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public ExportMetadata FindExport(Type interfaceType, string name,  ErrorTracer errorTracer ,bool isOptional)
        {
            Contract.Requires<ArgumentNullException>(interfaceType != null, "interfaceType");
            Contract.Requires<ArgumentNullException>(errorTracer != null, "errorTracer");

            throw new NotImplementedException();
        }

        /// <summary>
        ///     The find export.
        /// </summary>
        /// <param name="interfaceType">
        ///     The interface type.
        /// </param>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <returns>
        ///     The <see cref="ExportMetadata" />.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public ExportMetadata FindExport(Type interfaceType, string name, ErrorTracer errorTracer)
        {
            Contract.Requires<ArgumentNullException>(interfaceType != null, "interfaceType");
            Contract.Requires<ArgumentNullException>(errorTracer != null, "errorTracer");

            throw new NotImplementedException();
        }

        /// <summary>
        ///     The find exports.
        /// </summary>
        /// <param name="interfaceType">
        ///     The interface type.
        /// </param>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <returns>
        ///     The <see cref="IEnumerable" />.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public IEnumerable<ExportMetadata> FindExports(Type interfaceType, string name, ErrorTracer errorTracer)
        {
            Contract.Requires<ArgumentNullException>(interfaceType != null, "interfaceType");
            Contract.Requires<ArgumentNullException>(errorTracer != null, "errorTracer");

            throw new NotImplementedException();
        }

        public ExportMetadata FindExport(Type interfaceType, string name, ErrorTracer errorTracer, bool isOptional, int level)
        {
            Contract.Requires<ArgumentNullException>(interfaceType != null, "interfaceType");
            Contract.Requires<ArgumentNullException>(errorTracer != null, "errorTracer");

            throw new NotImplementedException();
        }

        public IEnumerable<ExportMetadata> FindExports(Type interfaceType, string name, ErrorTracer errorTracer, int level)
        {
            Contract.Requires<ArgumentNullException>(interfaceType != null, "interfaceType");
            Contract.Requires<ArgumentNullException>(errorTracer != null, "errorTracer");

            throw new NotImplementedException();
        }

        /// <summary>
        ///     The register.
        /// </summary>
        /// <param name="exportType">
        ///     The export type.
        /// </param>
        /// <param name="level"></param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public void Register(IExport exportType, int level)
        {
            Contract.Requires<ArgumentNullException>(exportType != null, "exportType");

            throw new NotImplementedException();
        }

        /// <summary>
        ///     The register.
        /// </summary>
        /// <param name="exportResolver">
        ///     The export resolver.
        /// </param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public void Register(ExportResolver exportResolver)
        {
            Contract.Requires<ArgumentNullException>(exportResolver != null, "exportResolver");

            throw new NotImplementedException();
        }

        /// <summary>
        ///     The register.
        /// </summary>
        /// <param name="extension">
        ///     The extension.
        /// </param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public void Register(IContainerExtension extension)
        {
            Contract.Requires<ArgumentNullException>(extension != null, "extension");

            throw new NotImplementedException();
        }

        #endregion
    }
}