using System;
using Tauron.Application.Files.Serialization.Core;
using Tauron.Application.Files.Serialization.Sources;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization
{
    [PublicAPI]
    public sealed class TypedSerializer<TType>
        where TType : class 
    {
        private readonly ISerializer _serializer;

        public ISerializer Serializer => _serializer;

        public TypedSerializer([NotNull] ISerializer serializer)
        {
            if (serializer == null) throw new ArgumentNullException(nameof(serializer));
            _serializer = serializer;
        }

        #region IStreamSource

        public void Serialize([NotNull] IStreamSource source, [NotNull] TType graph)
        {
            _serializer.Serialize(source, graph);
        }

        public void Deserialize([NotNull] IStreamSource source, [NotNull] TType target)
        {
            _serializer.Deserialize(source, target);
        }

        [NotNull]
        public TType Deserialize([NotNull] IStreamSource source)
        {
            return (TType) _serializer.Deserialize(source);
        }

        #endregion

        #region Files

        public void Serialize([NotNull] string file, [NotNull] TType graph)
        {
            _serializer.Serialize(new FileSource(file), graph);
        }

        public void Deserialize([NotNull] string file, [NotNull] TType target)
        {
            _serializer.Deserialize(new FileSource(file), target);
        }

        [NotNull]
        public TType Deserialize([NotNull] string file)
        {
            return (TType)_serializer.Deserialize(new FileSource(file));
        }

        #endregion

    }
}
