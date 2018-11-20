using System.Collections.Generic;
using JetBrains.Annotations;

namespace Tauron.Application.Common.CastleProxy.Impl
{
    public abstract class MetadataBase
    {
        protected MetadataBase([NotNull] IDictionary<string, object> metadata) => Metadata = new Dictionary<string, object>(Argument.NotNull(metadata, nameof(metadata)));

        public Dictionary<string, object> Metadata { get; }
    }
}