// The file SimpleResolver.cs is part of Tauron.Application.Common.
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
// <copyright file="SimpleResolver.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The simple resolver.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.Contracts;
using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    public delegate bool InterceptorCallback(ref object value);

    /// <summary>The simple resolver.</summary>
    [PublicAPI]
    public class SimpleResolver : IResolver
    {
        private class ExportFactoryHelper
        {
            private readonly ExportMetadata _buildMetadata;
            private readonly IContainer _container;
            private readonly object _metadataObject;
            private readonly InterceptorCallback _interceptor;

            public ExportFactoryHelper([NotNull] IContainer container, [NotNull] ExportMetadata buildMetadata,
                                       [NotNull] object metadataObject, [CanBeNull] InterceptorCallback interceptor)
            {
                _container = container;
                _buildMetadata = buildMetadata;
                _metadataObject = metadataObject;
                _interceptor = interceptor;
            }

            [CanBeNull]
            public object BuildUp([CanBeNull] BuildParameter[] parameters)
            {
                var error = new ErrorTracer();

                var temp = _container.BuildUp(_buildMetadata, error, parameters);
                if (error.Exceptional) throw new BuildUpException(error);

                if (_interceptor == null) return temp;

                return _interceptor(ref temp) ? temp : null;
            }

            [NotNull]
            public object Metadata()
            {
                return _metadataObject;
            }
        }

        #region Fields

        private readonly IContainer _container;
        private readonly Type _factoryType;
        private readonly bool _isExportFactory;
        private readonly ExportMetadata _metadata;
        private readonly object _metadataObject;
        private readonly Type _metadataType;
        private readonly InterceptorCallback _interceptor;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SimpleResolver" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="SimpleResolver" /> Klasse.
        /// </summary>
        /// <param name="metadata">
        ///     The metadata.
        /// </param>
        /// <param name="container">
        ///     The container.
        /// </param>
        /// <param name="isExportFactory"></param>
        /// <param name="factoryType"></param>
        /// <param name="metadataObject"></param>
        /// <param name="metadataType"></param>
        /// <param name="interceptor"></param>
        public SimpleResolver([NotNull] ExportMetadata metadata, [NotNull] IContainer container,
                              bool isExportFactory, [CanBeNull] Type factoryType, [CanBeNull] object metadataObject,
                              [CanBeNull] Type metadataType, [CanBeNull] InterceptorCallback interceptor)
        {
            Contract.Requires<ArgumentNullException>(container != null, "container");
            Contract.Requires<ArgumentNullException>(!isExportFactory || metadataType != null, "metadataType");

            _metadata = metadata;
            _container = container;
            _isExportFactory = isExportFactory;
            _factoryType = factoryType;
            _metadataObject = metadataObject;
            _metadataType = metadataType;
            _interceptor = interceptor;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the container.</summary>
        [NotNull]
        public IContainer Container
        {
            get
            {
                Contract.Ensures(Contract.Result<IContainer>() != null);

                return _container;
            }
        }

        /// <summary>Gets the metadata.</summary>
        [NotNull]
        public ExportMetadata Metadata
        {
            get
            {
                Contract.Ensures(Contract.Result<ExportMetadata>() != null);

                return _metadata;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The create.</summary>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        public object Create(ErrorTracer errorTracer)
        {
            if (_metadata == null) return null;

            errorTracer.Phase = "Injecting Import For " + _metadata; 

            try
            {
                if (!_isExportFactory)
                {
                    object temp;
                    try
                    {
                        errorTracer.IncrementIdent();
                        temp = Container.BuildUp(Metadata, errorTracer);
                    }
                    finally
                    {
                        errorTracer.DecrementIdent();
                    }
                    if (errorTracer.Exceptional) return null;

                    if (_interceptor == null) return temp;

                    return _interceptor(ref temp) ? temp : null;
                }

                var helper = new ExportFactoryHelper(_container, _metadata, _metadataObject, _interceptor);

                return Activator.CreateInstance(typeof (InstanceResolver<,>).MakeGenericType(_factoryType, _metadataType),
                    new Func<BuildParameter[], object>(helper.BuildUp), new Func<object>(helper.Metadata), _metadata.Export.ImplementType);
            }
            catch(Exception e)
            {
                errorTracer.Exceptional = true;
                errorTracer.Exception = e;
                return null;
            }
        }

        #endregion
    }
}