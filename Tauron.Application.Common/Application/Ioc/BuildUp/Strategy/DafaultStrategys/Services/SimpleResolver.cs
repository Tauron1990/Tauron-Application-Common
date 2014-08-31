#region

using System;
using System.Diagnostics.Contracts;
using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    public delegate bool InterceptorCallback(ref object value);

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
        private readonly bool _isDescriptor;

        #endregion

        #region Constructors and Destructors

        public SimpleResolver([NotNull] ExportMetadata metadata, [NotNull] IContainer container,
                              bool isExportFactory, [CanBeNull] Type factoryType, [CanBeNull] object metadataObject,
                              [CanBeNull] Type metadataType, [CanBeNull] InterceptorCallback interceptor, bool isDescriptor)
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
            _isDescriptor = isDescriptor;
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

        public object Create([NotNull] ErrorTracer errorTracer)
        {
            if (_metadata == null) return null;

            errorTracer.Phase = "Injecting Import For " + _metadata;

            try
            {
                if (_isDescriptor) return new ExportDescriptor(_metadata);

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

                return
                    Activator.CreateInstance(typeof (InstanceResolver<,>).MakeGenericType(_factoryType, _metadataType),
                                             new Func<BuildParameter[], object>(helper.BuildUp),
                                             new Func<object>(helper.Metadata), _metadata.Export.ImplementType);
            }
            catch (Exception e)
            {
                errorTracer.Exceptional = true;
                errorTracer.Exception = e;
                return null;
            }
        }

        #endregion
    }
}