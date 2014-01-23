using System;
using Tauron.Application.Files.Serialization.Core;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization
{
    [PublicAPI]
    public interface ISerializer
    {
        [CanBeNull]
        AggregateException Errors { get; }

        void Serialize([NotNull] IStreamSource target, [NotNull] object graph);

        [NotNull]
        object Deserialize([NotNull] IStreamSource target);
        void Deserialize([NotNull] IStreamSource targetStream, [NotNull] object target);
    }
}
