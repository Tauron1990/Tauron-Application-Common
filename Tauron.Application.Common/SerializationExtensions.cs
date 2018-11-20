using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using JetBrains.Annotations;

namespace Tauron
{
    [PublicAPI]
    public static class SerializationExtensions
    {
        private class XmlSerilalizerDelegator : IFormatter
        {
            private readonly XmlSerializer _serializer;

            public XmlSerilalizerDelegator([NotNull] XmlSerializer serializer)
            {
                Argument.NotNull(serializer, nameof(serializer));
                _serializer = serializer;
            }
            [CanBeNull]
            public SerializationBinder Binder
            {
                get => null;

                set { }
            }

            public StreamingContext Context
            {
                get => new StreamingContext();

                set { }
            }

            [CanBeNull]
            public ISurrogateSelector SurrogateSelector
            {
                get => null;

                set { }
            }
            [NotNull]
            public object Deserialize([NotNull] Stream serializationStream) => _serializer.Deserialize(serializationStream);

            public void Serialize([NotNull] Stream serializationStream, [NotNull] object graph) => _serializer.Serialize(serializationStream, graph);
        }

        [NotNull]
        public static TValue Deserialize<TValue>([NotNull] this string path, [NotNull] IFormatter formatter)
            where TValue : class
        {
            Argument.NotNull(path, nameof(path));
            Argument.NotNull(formatter, nameof(formatter));
            using (var stream = path.OpenRead())
                return (TValue) InternalDeserialize(formatter, stream);
        }

        [NotNull]
        public static TValue Deserialize<TValue>([NotNull] this string path)
            where TValue : class
        {
            Argument.NotNull(path, nameof(path));
            if (!path.ExisFile()) return Activator.CreateInstance<TValue>();

            using (var stream = path.OpenRead())
                return (TValue) InternalDeserialize(new BinaryFormatter(), stream);
        }

        public static void Serialize([NotNull] this object graph, [NotNull] IFormatter formatter, [NotNull] string path)
        {
            Argument.NotNull(graph, nameof(graph));
            Argument.NotNull(formatter, nameof(formatter));
            Argument.NotNull(path, path);

            using (var stream = path.OpenWrite())
                InternalSerialize(graph, formatter, stream);
        }

        public static void Serialize([NotNull] this object graph, [NotNull] string path)
        {
            Argument.NotNull(graph, nameof(path));
            Argument.NotNull(path, nameof(path));
            path.CreateDirectoryIfNotExis();

            using (var stream = path.OpenWrite())
                InternalSerialize(graph, new BinaryFormatter(), stream);
        }

        [NotNull]
        public static TValue XmlDeserialize<TValue>([NotNull] this string path, [NotNull] XmlSerializer formatter)
            where TValue : class
        {
            Argument.NotNull(path, nameof(path));
            Argument.NotNull(formatter, nameof(formatter));

            using (var stream = path.OpenRead())
                return (TValue) InternalDeserialize(new XmlSerilalizerDelegator(formatter), stream);
        }

        [NotNull]
        public static TValue XmlDeserializeIfExis<TValue>([NotNull] this string path, [NotNull] XmlSerializer formatter)
            where TValue : class
        {
            Argument.NotNull(path, nameof(path));
            Argument.NotNull(formatter, nameof(formatter));

            return path.ExisFile() ? XmlDeserialize<TValue>(path, formatter) : Activator.CreateInstance<TValue>();
        }

        public static void XmlSerialize([NotNull] this object graph, [NotNull] XmlSerializer formatter, [NotNull] string path)
        {
            Argument.NotNull(graph, nameof(graph));
            Argument.NotNull(formatter, nameof(formatter));
            Argument.NotNull(path, nameof(path));

            using (var stream = path.OpenWrite())
                InternalSerialize(graph, new XmlSerilalizerDelegator(formatter), stream);
        }

        
        [NotNull]
        private static object InternalDeserialize([NotNull] IFormatter formatter, [NotNull] Stream stream) => formatter.Deserialize(stream);

        private static void InternalSerialize([NotNull] object graph, [NotNull] IFormatter formatter, [NotNull] Stream stream) => formatter.Serialize(stream, graph);
    }
}