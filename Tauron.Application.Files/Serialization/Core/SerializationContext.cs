using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core
{
    [PublicAPI]
    public sealed class SerializationContext : IDisposable
    {
        private class GenericSource : IStreamSource
        {
            private byte[] _bytes;
            private SerializationContext _original;

            public GenericSource([NotNull] byte[] bytes, [NotNull] SerializationContext original)
            {
                _bytes = bytes;
                _original = original;
            }

            public void Dispose()
            {
                _bytes = null;
                _original = null;
            }

            public Stream OpenStream(FileAccess access)
            {
                return new MemoryStream(_bytes);
            }

            public IStreamSource OpenSideLocation(string relativePath)
            {
                return _original.StreamSource.OpenSideLocation(relativePath);
            }
        }

        private class BackgroundStream
        {
            [CanBeNull]
            public Stream Stream { get; set; }
        }

        private Dictionary<string, SerializationContext> _childContexts;
        private BackgroundStream _backgroundStream;
        private bool _snapShot;

        public bool IsSnapShot => _snapShot;
        public ContextMode ContextMode { get; private set; }
        [NotNull]
        public IStreamSource StreamSource { get; private set; }
        public SerializerMode SerializerMode { get; private set; }

        public SerializationContext(ContextMode contextMode, [NotNull] IStreamSource streamSource, SerializerMode serializerMode)
        {
            ContextMode = contextMode;
            StreamSource = streamSource;
            SerializerMode = serializerMode;
            _backgroundStream = new BackgroundStream();
        }

        private SerializationContext(ContextMode contextMode, [NotNull] IStreamSource streamSource,
                                     SerializerMode serializerMode, [NotNull] BackgroundStream parentStream)
        {
            ContextMode = contextMode;
            StreamSource = streamSource;
            SerializerMode = serializerMode;
            _backgroundStream = parentStream;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Objekte verwerfen, bevor Bereich verloren geht", Justification = "No Disposing Needed"), CanBeNull]
        public SerializationContext GetSubContext([CanBeNull] string relativeFile, ContextMode contextMode)
        {
            if (relativeFile == null) return this;

            SerializationContext context;
            if (_childContexts.TryGetValue(relativeFile, out context)) return context;

            IStreamSource streamSource = StreamSource.OpenSideLocation(relativeFile);
            if (streamSource == null) return null;

            context = new SerializationContext(contextMode, streamSource, SerializerMode, _backgroundStream);
            _childContexts[relativeFile] = context;

            return context;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Objekte verwerfen, bevor Bereich verloren geht", Justification = "No Disposing Needed"), NotNull]
        public SerializationContext CreateSnapshot([NotNull] string value)
        {
            return new SerializationContext(ContextMode, new GenericSource(Encoding.UTF8.GetBytes(value), this), SerializerMode) { _snapShot = true };
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Objekte verwerfen, bevor Bereich verloren geht", Justification = "No disposing Needed"), NotNull]
        public SerializationContext CreateSnapshot([NotNull] byte[] value)
        {
            return new SerializationContext(ContextMode, new GenericSource(value, this), SerializerMode) { _snapShot = true };
        }

        [NotNull]
        private Stream EnsurceBackgroundStream()
        {
            if (_snapShot) return StreamSource.OpenStream(FileAccess.ReadWrite);

            return _backgroundStream.Stream ??
                   (_backgroundStream.Stream = StreamSource.OpenStream(SerializerMode == SerializerMode.Deserialize
                                                                    ? FileAccess.Read
                                                                    : FileAccess.Write));
        }

        #region Binary

        private BinaryReader _binaryReader;

        [NotNull]
        public BinaryReader BinaryReader
        {
            get
            {
                if (ContextMode != ContextMode.Binary && SerializerMode != SerializerMode.Deserialize && !_snapShot) throw new InvalidOperationException("No Binary Deserialisation");

                return _binaryReader ?? (_binaryReader = new BinaryReader(EnsurceBackgroundStream()));
            }
        }

        private BinaryWriter _binaryWriter;

        [NotNull]
        public BinaryWriter BinaryWriter
        {
            get
            {
                if (ContextMode != ContextMode.Binary && SerializerMode != SerializerMode.Serialize && !_snapShot) throw new InvalidOperationException("No Binary Serialisation");

                return _binaryWriter ?? (_binaryWriter = new BinaryWriter(EnsurceBackgroundStream()));
            }
        }

        public void WriteBytes([NotNull] byte[] value)
        {
            if(value == null) return;

            BinaryWriter.Write(value.Length);
            BinaryWriter.Write(value);
        }

        [NotNull]
        public byte[] Readbytes()
        {
            return BinaryReader.ReadBytes(BinaryReader.ReadInt32());
        }

        #endregion

        #region Text

        private TextReader _textReader;

        [NotNull]
        public TextReader TextReader
        {
            get
            {
                if (ContextMode != ContextMode.Binary && SerializerMode != SerializerMode.Deserialize) throw new InvalidOperationException("No Binary Deserialisation");

                return _textReader ?? (_textReader = new StreamReader(EnsurceBackgroundStream()));
            }
        }

        private TextWriter _textWriter;

        [NotNull]
        public TextWriter TextWriter
        {
            get
            {
                if (ContextMode != ContextMode.Binary && SerializerMode != SerializerMode.Serialize) throw new InvalidOperationException("No Binary Serialisation");

                return _textWriter ?? (_textWriter = new StreamWriter(EnsurceBackgroundStream()));
            }
        }

        #endregion

        [NotNull]
        public static string ConvertByteToString([NotNull] byte[] value)
        {
            return Base91.Encode(value);
        }

        [NotNull]
        public static byte[] ConvertStringToBytes([NotNull] string value)
        {
            return Base91.Decode(value);
        }

        public void Dispose()
        {
            DisposeImpl(true);
            GC.SuppressFinalize(this);
        }

        ~SerializationContext()
        {
            DisposeImpl(false);
        }

        private void DisposeImpl(bool disposing)
        {
            if (disposing && _childContexts != null)
            {
                StreamSource.Dispose();

                foreach (var serializationContext in _childContexts)
                {
                    serializationContext.Value.Dispose();
                }
            }

            if (_backgroundStream != null && _backgroundStream.Stream != null)
            {
                _backgroundStream.Stream.Dispose();
            }

            _backgroundStream = null;

            if (_binaryWriter != null)
            {
                _binaryWriter.Dispose();
                _binaryWriter = null;
            }
            if (_binaryReader != null)
            {
                _binaryReader.Dispose();
                _binaryReader = null;
            }
            if (_textReader != null)
            {
                _textReader.Dispose();
                _textReader = null;
            }

            if (_textWriter == null) return;
            _textWriter.Dispose();
            _textWriter = null;
        }
    }
}