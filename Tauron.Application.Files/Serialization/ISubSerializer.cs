using Tauron.Application.Files.Serialization.Core;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization
{
    public interface ISubSerializer : ISerializer
    {
        void Serialize([NotNull] SerializationContext target, [NotNull] object graph);
        [NotNull]
        object Deserialize([NotNull] SerializationContext target);
    }
}