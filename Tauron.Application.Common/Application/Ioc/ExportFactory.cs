using System;
using System.Diagnostics.Contracts;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Ioc
{
    [PublicAPI]
    public class InstanceResolver<TExport, TMetadata>
        where TMetadata : class
    {
        private readonly Func<object> _resolver;

        private TMetadata _metadata;
        private Func<object> _metadataFactory;

        public InstanceResolver([NotNull] Func<object> resolver, [NotNull] Func<object> metadataFactory,
                                [NotNull] Type realType)
        {
            Contract.Requires<ArgumentNullException>(resolver != null, "resolver");
            Contract.Requires<ArgumentNullException>(metadataFactory != null, "metadataFactory");
            Contract.Requires<ArgumentNullException>(realType != null, "realType");

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

        public TExport Resolve()
        {
            object obj = _resolver();

            if (obj == null) return default(TExport);

            return (TExport) obj;
        }

        public TExport Resolve(bool uiSync)
        {
            return UiSynchronize.Synchronize.Invoke<TExport>(Resolve);
        }
    }
}