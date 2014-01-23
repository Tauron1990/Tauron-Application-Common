using System.Collections.Generic;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Managment
{
    public class SimpleMapper<TContext>
        where TContext : IOrginalContextProvider
    {
        public SimpleMapper()
        {
            Entries = new List<MappingEntry<TContext>>();
        }

        [NotNull]
        public List<MappingEntry<TContext>> Entries { get; private set; }
    }
}
