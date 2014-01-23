using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.HeaderedText
{
    [PublicAPI]
    public sealed class HeaderedFileWriter
    {
        private readonly HeaderedFile _file;
        private readonly FileContext _context;
        private readonly FileDescription _description;

        private bool _isWriten;

        internal HeaderedFileWriter([NotNull] HeaderedFile file)
        {
            _file = file;
            _file.CurrentWriter = this;
            _context = file.Context;
            _description = _context.Description;
        }

        [CanBeNull]
        public string Content
        {
            get { return _file.Content; } 
            set { _file.Content = value; }
        }

        public void Add(string key, string value)
        {
            if(!_description.Contains(key))
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "The key {0} is Invalid", key));

            _context.Add(new ContextEnry(key, value));
        }

        [NotNull]
        public IEnumerable<ContextEnry> Enries { get { return _context; } } 

        public IEnumerable<ContextEnry> this[string key]
        {
            get { return _context[key]; }
        }

        public bool Remove([NotNull] ContextEnry entry)
        {
            return _context.ContextEnries.Remove(entry);
        }

        public int RemoveAll([NotNull] string key)
        {
            return _context.ContextEnries.RemoveAll(ent => ent.Key == key);
        }

        public void Save([NotNull] TextWriter writer)
        {
            if (writer == null) throw new ArgumentNullException("writer");
            if (_isWriten) throw new InvalidOperationException("The Content is Writen");

            _context.ContextEnries
                    .Sort((one, two) => String.Compare(one.Key, two.Key, StringComparison.Ordinal));

            foreach (var contextEnry in Enries) writer.WriteLine("{0} {1}", contextEnry.Key, contextEnry.Content);

            writer.Write(Content);
            writer.Flush();

            _file.CurrentWriter = null;
            _isWriten = true;
        }
    }
}
