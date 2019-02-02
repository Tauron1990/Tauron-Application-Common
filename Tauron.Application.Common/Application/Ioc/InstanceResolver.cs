using System;
using JetBrains.Annotations;

namespace Tauron.Application.Ioc
{
    [PublicAPI]
    public sealed class InstanceResolver<TExport, TMetadata>
        where TMetadata : class
    {
        private readonly Func<BuildParameter[], object> _resolver;

        private TMetadata _metadata;
        private Func<object> _metadataFactory;

        public InstanceResolver([NotNull] Func<BuildParameter[], object> resolver, [NotNull] Func<object> metadataFactory, [NotNull] Type realType)
        {
            Argument.NotNull(resolver, nameof(resolver));
            Argument.NotNull(metadataFactory, nameof(metadataFactory));
            Argument.NotNull(realType, nameof(realType));

            RealType = realType;
            _resolver = resolver;
            _metadataFactory = metadataFactory;
        }


        [NotNull]
        public Type RealType { get; private set; }

        [NotNull]
        public TMetadata Metadata
        {
            get
            {
                if (_metadata != null) return _metadata;

                _metadata = (TMetadata) _metadataFactory();
                _metadataFactory = null;

                return _metadata;
            }
        }

        public TExport Resolve([CanBeNull] BuildParameter[] buildParameters = null)
        {
            var obj = _resolver(buildParameters);

            if (obj == null) return default;

            return (TExport) obj;
        }

        public TExport Resolve(bool uiSync, [CanBeNull] BuildParameter[] buildParameters = null)
        {
            return uiSync
                ? UiSynchronize.Synchronize.Invoke(() => Resolve(buildParameters))
                : Resolve(buildParameters);
        }

        public object ResolveRaw([CanBeNull] BuildParameter[] buildParameters = null)
        {
            var obj = _resolver(buildParameters);

            return obj;
        }

        public object ResolveRaw(bool uiSync, [CanBeNull] BuildParameter[] buildParameters = null)
        {
            return uiSync
                ? UiSynchronize.Synchronize.Invoke(() => ResolveRaw(buildParameters))
                : ResolveRaw(buildParameters);
        }
    }
}