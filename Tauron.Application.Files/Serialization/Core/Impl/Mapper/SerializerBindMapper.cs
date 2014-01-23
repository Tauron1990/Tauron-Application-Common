using System;
using System.IO;
using Tauron.Application.Files.Serialization.Core.Managment;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Impl.Mapper
{
    internal class SerializerBindMapper<TContext> : MappingEntryBase<TContext>
        where TContext : IOrginalContextProvider
    {
        private readonly ISerializer _serializer;
        private readonly StreamManager _manager;

        public SerializerBindMapper([CanBeNull] string membername, [NotNull] Type targetType, [CanBeNull] ISerializer serializer, 
            [CanBeNull] StreamManager manager) : base(membername, targetType)
        {
            _serializer = serializer;
            _manager = manager;
        }

        protected override void Deserialize(object target, TContext context)
        {
            using (_manager)
            {
                _manager.Initialize(target, SerializerMode.Deserialize);
                SetValue(target, _serializer.Deserialize(_manager));
            }
        }

        protected override void Serialize(object target, TContext context)
        {
            using (_manager)
            {
                _manager.Initialize(target, SerializerMode.Serialize);
                _serializer.Serialize(_manager, GetValue(target));
            }
        }

        public override Exception VerifyError()
        {
            Exception e = base.VerifyError();

            if (e != null) return e;

            e = _manager == null ? new ArgumentNullException("Open Stream") : _manager.VerifyError();

            if(_serializer == null)
                e = new ArgumentNullException("Serializer");

            return e;
        }

        internal class StreamManager : IStreamSource
        {
            private readonly Func<object, SerializerMode, Stream> _open;
            private readonly Func<string, IStreamSource> _openRelative;

            private Stream _current;

            public StreamManager([CanBeNull] Func<object, SerializerMode, Stream> open, [CanBeNull] Func<string, IStreamSource> openRelative)
            {
                _open = open;
                _openRelative = openRelative;
            }

            [NotNull]
            private Stream OpenStream([NotNull] object target, SerializerMode mode)
            {
                if (_current != null) throw new InvalidOperationException();

                _current = _open(target, mode);
                return _current;
            }

            public void Dispose()
            {
                if(_current == null)
                    return;
                
                _current.Dispose();
                _current = null;
                _target = null;
            }

            [CanBeNull]
            public Exception VerifyError()
            {
                return _open == null ? new SerializerElementException("Open Function") : null;
            }

            private object _target;
            private SerializerMode _mode;

            public void Initialize([NotNull] object target, SerializerMode mode)
            {
                _target = target;
                _mode = mode;
            }

            public Stream OpenStream(FileAccess access)
            {
                return OpenStream(_target, _mode);
            }

            public IStreamSource OpenSideLocation(string relativePath)
            {
                if (_openRelative != null) return _openRelative(relativePath);

                throw new NotSupportedException();
            }
        }
    }
}
