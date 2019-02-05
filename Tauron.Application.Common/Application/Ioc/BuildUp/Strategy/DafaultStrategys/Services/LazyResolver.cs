using System;
using System.Collections.Generic;
using System.Threading;
using ExpressionBuilder;
using ExpressionBuilder.Fluent;
using FastExpressionCompiler;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    /// <summary>The lazy resolver.</summary>
    public class LazyResolver : IResolver
    {
        public LazyResolver(SimpleResolver resolver, Type lazy, IMetadataFactory factory)
        {
            _resolver = resolver;
            _lazy = lazy;
            _factory = factory;
        }

        public IRightable Create(ErrorTracer errorTracer, CompilationUnit unit) 
            => CreateLazy(_lazy, _factory, _resolver.Metadata.Metadata ?? new Dictionary<string, object>(), _resolver, errorTracer);

        private static IRightable CreateLazy([NotNull] Type lazytype, [NotNull] IMetadataFactory metadataFactory, [NotNull] IDictionary<string, object> metadataValue,
            [NotNull] SimpleResolver creator, [NotNull] ErrorTracer errorTracer)
        {
            Argument.NotNull(lazytype, nameof(lazytype));
            Argument.NotNull(metadataFactory, nameof(metadataFactory));
            Argument.NotNull(metadataValue, nameof(metadataValue));
            Argument.NotNull(creator, nameof(creator));
            Argument.NotNull(errorTracer, nameof(errorTracer));

            errorTracer.Phase = "Injecting Lazy For " + lazytype.Name;

            try
            {
                var openGeneric = lazytype.GetGenericTypeDefinition();

                var trampolineBase = typeof(LazyTrampoline<>);
                var trampolineGenerics = new Type[1];
                trampolineGenerics[0] = lazytype.GenericTypeArguments[0];

                var trampoline = trampolineBase.MakeGenericType(trampolineGenerics);

                var trampolineImpl = Operation.CreateInstance(trampoline, Operation.Constant(creator));
                var metadata = openGeneric == InjectorBaseConstants.Lazy ? null : lazytype.GenericTypeArguments[1];
                var valueFac = Operation.Cast(
                    Operation.InvokeReturn(trampolineImpl, nameof(LazyTrampolineBase.CreateFunc)), typeof(Func<>).MakeGenericType(lazytype.GenericTypeArguments[0]));

                if (metadata == null) return Operation.CreateInstance(lazytype, valueFac);

                return Operation.CreateInstance(lazytype,
                    valueFac,
                    Operation.InvokeReturn(Operation.Constant(metadataFactory), nameof(metadataFactory.CreateMetadata),
                        Operation.Constant(metadata), Operation.Constant(metadataValue)));
            }
            catch (Exception e)
            {
                errorTracer.Exceptional = true;
                errorTracer.Exception = e;
                return null;
            }
        }

        private class LazyTrampoline<T> : LazyTrampolineBase
        {
            private readonly SimpleResolver _resolver;

            public LazyTrampoline([NotNull] SimpleResolver resolver) => _resolver = Argument.NotNull(resolver, nameof(resolver));

            public override object CreateFunc() => Create();

            private Func<T> Create()
            {
                string returnVar = "ReturnVar_" + Interlocked.Increment(ref PrivateVariableStade);
                var f = Function.Create("SimpleResolver")
                    .WithBody(
                        CodeLine.CreateVariable<T>(returnVar),
                        CodeLine.Assign(Operation.Variable(returnVar), Operation.Cast(_resolver.Create(new ErrorTracer(), null), typeof(T))))
                    .Returns(returnVar)
                    .ToExpression().CompileFast<Func<T>>();

                return f;
            }
        }

        private abstract class LazyTrampolineBase
        {
            protected static int PrivateVariableStade;

            public abstract object CreateFunc();

        }

        private readonly IMetadataFactory _factory;

        private readonly Type _lazy;

        private readonly SimpleResolver _resolver;
    }
}