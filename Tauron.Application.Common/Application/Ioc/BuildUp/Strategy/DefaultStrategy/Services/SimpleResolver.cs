﻿using System;
using System.Linq;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DefaultStrategy
{
    public delegate bool InterceptorCallback(ref object value);

    [PublicAPI]
    public class SimpleResolver : IResolver
    {
        public SimpleResolver([NotNull] ExportMetadata metadata, [NotNull] IContainer container,
            bool isExportFactory, [CanBeNull] Type factoryType, [CanBeNull] object metadataObject,
            [CanBeNull] Type metadataType, [CanBeNull] InterceptorCallback interceptor, bool isDescriptor,
            [NotNull] IResolverExtension[] extensions)
        {
            Metadata = Argument.NotNull(metadata, nameof(metadata));
            Container = Argument.NotNull(container, nameof(container));
            _isExportFactory = isExportFactory;
            _factoryType = factoryType;
            _metadataObject = metadataObject;
            _metadataType = metadataType;
            _interceptor = interceptor;
            _isDescriptor = isDescriptor;
            _extensions = Argument.NotNull(extensions, nameof(extensions));
        }

        public object Create([NotNull] ErrorTracer errorTracer)
        {
            errorTracer.Phase = "Injecting Import For " + Metadata;

            var helper = new ExportFactoryHelper(Container, Metadata, _metadataObject, _interceptor, _extensions);

            try
            {
                if (_isDescriptor) return new ExportDescriptor(Metadata);

                if (_isExportFactory)
                {
                    return typeof(InstanceResolver<,>).MakeGenericType(_factoryType, _metadataType).FastCreateInstance(
                        new Func<BuildParameter[], object>(helper.BuildUp),
                        new Func<object>(helper.Metadata), Metadata.Export.ImplementType);
                }

                try
                {
                    errorTracer.IncrementIdent();
                    return helper.BuildUp(null, errorTracer);
                }
                finally
                {
                    errorTracer.DecrementIdent();
                }
            }
            catch (Exception e)
            {
                errorTracer.Exceptional = true;
                errorTracer.Exception = e;
                return null;
            }
        }

        private class ExportFactoryHelper
        {
            private readonly ExportMetadata _buildMetadata;
            private readonly IContainer _container;
            private readonly IResolverExtension[] _extensions;
            private readonly InterceptorCallback _interceptor;
            private readonly object _metadataObject;

            public ExportFactoryHelper([NotNull] IContainer container, [NotNull] ExportMetadata buildMetadata,
                [NotNull] object metadataObject, [CanBeNull] InterceptorCallback interceptor,
                [NotNull] IResolverExtension[] extensions)
            {
                _container = container;
                _buildMetadata = buildMetadata;
                _metadataObject = metadataObject;
                _interceptor = interceptor;
                _extensions = extensions;
            }

            [CanBeNull]
            public object BuildUp([CanBeNull] BuildParameter[] parameters) => BuildUp(parameters, null);

            [CanBeNull]
            public object BuildUp([CanBeNull] BuildParameter[] parameters, [CanBeNull] ErrorTracer error)
            {
                if (error == null)
                    error = new ErrorTracer();

                var temp = _container.BuildUp(_buildMetadata, error, parameters);
                if (error.Exceptional) throw new BuildUpException(error);

                var effectiveType = temp.GetType();
                var extension = _extensions.FirstOrDefault(e => e.TargetType == effectiveType);
                if (extension != null)
                    temp = extension.Progress(_buildMetadata, temp);

                var flag = _interceptor == null || _interceptor(ref temp);

                return !flag ? null : temp;
            }

            [NotNull]
            public object Metadata() => _metadataObject;
        }

        private readonly Type _factoryType;
        private readonly bool _isExportFactory;
        private readonly object _metadataObject;
        private readonly Type _metadataType;
        private readonly InterceptorCallback _interceptor;
        private readonly bool _isDescriptor;
        private readonly IResolverExtension[] _extensions;
        
        public IContainer Container { get; }
        
        [NotNull]
        public ExportMetadata Metadata { get; }
    }
}