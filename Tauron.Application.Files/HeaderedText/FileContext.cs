using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.HeaderedText
{
    [PublicAPI]
    public sealed class FileContext : IEnumerable<ContextEnry>
    {
        private readonly FileDescription _description;

        private List<ContextEnry> _contextEnries = new List<ContextEnry>();

        [NotNull]
        internal FileDescription Description { get { return _description; } }

        [NotNull]
        internal List<ContextEnry> ContextEnries { get { return _contextEnries; } } 

        internal FileContext([NotNull] FileDescription description)
        {
            _description = (FileDescription) description.Clone();
        }

        public IEnumerable<ContextEnry> this[string key]
        {
            get 
            {
                return _contextEnries.Where(contextEnry => contextEnry.Key == key);
            }
        }

        internal void Reset()
        {
            _contextEnries.Clear();
        }

        internal bool IsKeyword([NotNull] string key)
        {
            return _description.Contains(key);
        }

        internal void Add([NotNull] ContextEnry entry)
        {
            _contextEnries.Add(entry);
        }

        public IEnumerator<ContextEnry> GetEnumerator()
        {
            return _contextEnries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
