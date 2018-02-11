using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Impl
{
    internal sealed class BinarySerializer : ISerializer
    {
        private readonly BinaryFormatter _formatter;

        public BinarySerializer([NotNull] BinaryFormatter formatter)
        {
            _formatter = formatter;
        }

        public AggregateException Errors => _formatter == null ? new AggregateException(new SerializerElementNullException("Formatter")) : null;

        public void Serialize(IStreamSource target, object graph)
        {
            if (target == null) throw new ArgumentNullException("target");
            if (graph == null) throw new ArgumentNullException("graph");

            using (var stream = target.OpenStream(FileAccess.ReadWrite))
                _formatter.Serialize(stream, graph);
        }

        public object Deserialize(IStreamSource target)
        {
            if (target == null) throw new ArgumentNullException("target");

            using (var stream = target.OpenStream(FileAccess.ReadWrite))
                return _formatter.Deserialize(stream);
        }

        public void Deserialize(IStreamSource targetStream, object target)
        {
            throw new NotSupportedException();
        }
    }
}
