using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Tauron.Application.Files.Serialization.Core.Managment;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Impl.Mapper
{
    [PublicAPI]
    public abstract class ContextImplBase : IOrginalContextProvider, IDisposable
    {
        private class ConsistentTextWriter : TextWriter
        {
            private readonly BinaryWriter _writer;
            private readonly List<string> _lines;
            private readonly StringBuilder _stringBuilder;

            public ConsistentTextWriter([NotNull] BinaryWriter writer)
                : base(CultureInfo.InvariantCulture)
            {
                _writer = writer;
                _lines = new List<string>();
                _stringBuilder = new StringBuilder();
            }

            public override Encoding Encoding
            {
                get
                {
                    return Encoding.UTF8;
                }
            }

            public override void Write(char value)
            {
                if (value == '\n')
                {
                    _lines.Add(_stringBuilder.ToString());
                    _stringBuilder.Clear();
                }

                _stringBuilder.Append(value);
            }

            protected override void Dispose(bool disposing)
            {
                _writer.Write(_lines.Count);
                foreach (var line in _lines)
                {
                    _writer.Write(line);
                }

                base.Dispose(disposing);
            }
        }
        private class ConsistentTextReader : TextReader
        {
            private readonly BinaryReader _reader;
            private readonly int _lines;

            private int _currentLine;
            private string _currentLineString;
            private int _currentLinePosition;

            public ConsistentTextReader([NotNull] BinaryReader reader)
            {
                _reader = reader;
                _lines = reader.ReadInt32();
                if (_lines == 0) return;

                _currentLine = 1;
                _currentLinePosition = 0;
                _currentLineString = reader.ReadString();
            }

            private bool MoveToNextLine()
            {
                _currentLine++;
                if (_currentLine > _lines) return false;

                _currentLinePosition = 0;
                _currentLineString = _reader.ReadString();
                return true;
            }

            private bool CheckPosition()
            {
                return _currentLinePosition < _currentLineString.Length || MoveToNextLine();
            }

            public override string ReadLine()
            {
                if (_currentLinePosition == 0 || (_currentLinePosition >= _currentLineString.Length && MoveToNextLine()))
                {
                    _currentLinePosition = _currentLineString.Length;
                    return _currentLineString;
                }

                string temp = _currentLineString.Substring(_currentLinePosition);
                _currentLinePosition = _currentLineString.Length;
                return temp;
            }

            public override string ReadToEnd()
            {
                var builder = new StringBuilder();

                if (_currentLinePosition == 0) builder.AppendLine(_currentLineString);
                else if (_currentLinePosition < _currentLineString.Length) builder.AppendLine(_currentLineString.Substring(_currentLinePosition));

                while (MoveToNextLine())
                {
                    builder.AppendLine(_currentLineString);
                }

                return builder.ToString();
            }

            public override int Peek()
            {
                if (CheckPosition()) return _currentLineString[_currentLinePosition];

                return -1;
            }

            public override int Read()
            {
                if (!CheckPosition()) return -1;

                int value = _currentLineString[_currentLinePosition];
                _currentLinePosition++;
                return value;
            }
        }

        public SerializationContext Original { get; private set; }

        protected ContextImplBase([NotNull] SerializationContext original)
        {
            if (original == null) throw new ArgumentNullException("original");
            Original = original;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(disposing)
                Original.Dispose();
        }

        ~ContextImplBase()
        {
            Dispose(false);
        }

        [NotNull]
        protected TextWriter TextWriter
        {
            get
            {
                return Original.ContextMode == ContextMode.Binary
                           ? new ConsistentTextWriter(Original.BinaryWriter)
                           : Original.TextWriter;
            }
        }

        [NotNull]
        protected TextReader TextReader
        {
            get
            {
                return Original.ContextMode == ContextMode.Text
                           ? Original.TextReader
                           : new ConsistentTextReader(Original.BinaryReader);
            }
        }
    }
}