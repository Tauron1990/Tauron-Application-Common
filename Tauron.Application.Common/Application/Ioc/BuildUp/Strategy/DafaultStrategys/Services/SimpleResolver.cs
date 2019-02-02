﻿using System;
using System.Linq;
using ExpressionBuilder;
using ExpressionBuilder.Fluent;
using ExpressionBuilder.Operations;
using FastExpressionCompiler;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    public delegate (Condition IsOK, ICodeLine Operation) InterceptorCallback(string variable);

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

        public IRightable Create([NotNull] ErrorTracer errorTracer)
        {
            errorTracer.Phase = "Injecting Import For " + Metadata;

            var helper = new ExportFactoryHelper(Container, Metadata, _metadataObject, _interceptor, _extensions);

            try
            {
                if (_isDescriptor) return Operation.CreateInstance(typeof(ExportDescriptor), Operation.Constant(Metadata));

                if (_isExportFactory)
                {
                    var fullType = typeof(InstanceResolver<,>).MakeGenericType(_factoryType, _metadataType);
                    var opBlock = helper.BuildUp(true);

                    return Operation.CreateInstance(fullType,
                        Operation.Constant(opBlock.Function.ToExpression().CompileFast<Func<BuildParameter, object>>()),
                        Operation.Constant(new Func<object>(helper.Metadata)),
                        Operation.Constant(new Func<object>(helper.Metadata)),
                        Operation.Constant(Metadata.Export.ImplementType));

                    //return
                    //    Activator.CreateInstance(
                    //        typeof(InstanceResolver<,>).MakeGenericType(_factoryType, _metadataType),
                    //        new Func<BuildParameter[], object>(helper.BuildUp),
                    //        new Func<object>(helper.Metadata), Metadata.Export.ImplementType);
                }

                try
                {
                    errorTracer.IncrementIdent();
                    return helper.BuildUp(false);
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

            //[CanBeNull]
            //public IRightable BuildUp() //([CanBeNull] BuildParameter[] parameters) => BuildUp(parameters, null);

            [NotNull]
            public OperationBlock BuildUp(bool hasParameter) //([CanBeNull] BuildParameter[] parameters, [CanBeNull] ErrorTracer error)
            {
                var op = Operation.NeestedLambda("creator", _buildMetadata.InterfaceType, parameter =>
                {
                    const string tempObject = "TempObject";
                    const string buildParameters = "buildParameters";

                    if (hasParameter)
                        parameter.WithParameter<BuildParameter[]>(buildParameters);

                    parameter
                        .WithBody(
                            CodeLine.CreateVariable<object>(tempObject),
                            CodeLine.Assign(tempObject, Operation.InvokeReturn(Operation.Constant(_container), nameof(_container.BuildUp), 
                                Operation.Constant(_buildMetadata), Operation.CreateInstance(typeof(ErrorTracer)), hasParameter 
                                    ? Operation.Variable(buildParameters) 
                                    : Operation.Null())),
                            _extensions.FirstOrDefault(e => e.TargetType == _buildMetadata.Export.ImplementType)?.Progress(_buildMetadata, tempObject),
                            CreateInterceptor(tempObject))
                        .Returns(tempObject);

                });

                return op;
            }

            [NotNull]
            public object Metadata() => _metadataObject;

            private ICodeLine CreateInterceptor(string variableName)
            {
                if (_interceptor == null) return CodeLine.Return();
                var erg = _interceptor(variableName);

                return CodeLine.CreateIf(erg.IsOK)
                    .Then(erg.Operation, CodeLine.Return())
                    .Else(CodeLine.Assign(variableName, Operation.Null()), CodeLine.Return());
            }
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