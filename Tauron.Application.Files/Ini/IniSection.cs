// The file IniSection.cs is part of Tauron.Application.Files.
// 
// CoreEngine is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// CoreEngine is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//  
// You should have received a copy of the GNU General Public License
//  along with Tauron.Application.Files If not, see <http://www.gnu.org/licenses/>.

#region

using System;
using System.Collections.Generic;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Files.Ini
{
    [PublicAPI, Serializable]
    public sealed class IniSection
    {
        private readonly Dictionary<string, IniEntry> _entries;

        public IniSection(Dictionary<string, IniEntry> entries, string name)
        {
            Name = name;
            _entries = entries;
        }

        public IniSection(string name)
            : this(new Dictionary<string, IniEntry>(), name)
        {
        }

        public ReadOnlyEnumerator<IniEntry> Entries => new ReadOnlyEnumerator<IniEntry>(_entries.Values);

        public string Name { get; private set; }

        public SingleIniEntry GetSingleEntry(string name)
        {
            IniEntry entry;
            if (_entries.TryGetValue(name, out entry))
                return entry as SingleIniEntry;
            return null;
        }

        public SingleIniEntry AddSingleKey(string name)
        {
            var entry = new SingleIniEntry(name, null);
            _entries[name] = entry;
            return entry;
        }

        public ListIniEntry GetListEntry(string name)
        {
            IniEntry value;
            if (!_entries.TryGetValue(name, out value)) return null;

            var multi = value as ListIniEntry;
            if (multi != null) return multi;

            multi = new ListIniEntry((SingleIniEntry) value);
            _entries[multi.Key] = multi;

            return multi;
        }

        public ListIniEntry GetOrAddListEntry(string name)
        {
            ListIniEntry entry = GetListEntry(name);
            if (entry != null)
                return entry;

            entry = new ListIniEntry(name, new List<string>());

            return entry;
        }

        /// <summary>
        ///     The get data.
        /// </summary>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <returns>
        ///     The <see cref="SingleIniEntry" />.
        /// </returns>
        public SingleIniEntry GetData(string name)
        {
            SingleIniEntry data = GetSingleEntry(name);

            if (data != null) return data;

            return AddSingleKey(name);
        }
    }
}