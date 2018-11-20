using System;
using System.Collections.Generic;
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

        public object Create(ErrorTracer errorTracer) => CreateLazy(_lazy, _factory, _resolver.Metadata.Metadata ?? new Dictionary<string, object>(), _resolver, errorTracer);

        private static object CreateLazy([NotNull] Type lazytype, [NotNull] IMetadataFactory metadataFactory, [NotNull] IDictionary<string, object> metadataValue,
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

                var trampolineImpl = (LazyTrampolineBase) Activator.CreateInstance(trampoline, creator);
                var metadata = openGeneric == InjectorBaseConstants.Lazy ? null : lazytype.GenericTypeArguments[1];

                if (metadata == null) return Activator.CreateInstance(lazytype, trampolineImpl.CreateFunc());

                return Activator.CreateInstance(
                    lazytype,
                    trampolineImpl.CreateFunc(),
                    metadataFactory.CreateMetadata(metadata, metadataValue));
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

            public override object CreateFunc() => (Func<T>) Create;

            private T Create() => (T) _resolver.Create(new ErrorTracer());
        }

        private abstract class LazyTrampolineBase
        {
            public abstract object CreateFunc();

        }
        private readonly IMetadataFactory _factory;

        private readonly Type _lazy;

        private readonly SimpleResolver _resolver;
    }
}