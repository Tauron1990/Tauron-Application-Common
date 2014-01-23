// The file IniParser.cs is part of Tauron.Application.Files.
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
using System.IO;
using System.Linq;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Files.Ini.Parser
{
    [PublicAPI]
    public sealed class IniParser
    {
        private static readonly char[] KeyValueChar = {'='};

        private readonly TextReader _reader;

        public IniParser([NotNull] TextReader reader)
        {
            if (reader == null) throw new ArgumentNullException("reader");

            _reader = reader;
        }

        [NotNull]
        public IniFile Parse()
        {
            var entrys = new Dictionary<string, GroupDictionary<string, string>>();
            var currentSection = new GroupDictionary<string, string>();
            string currentSectionName = null;

            foreach (var line in _reader.EnumerateTextLines())
            {
                if (line[0] == '[' && line[line.Length - 1] == ']')
                {
                    if (currentSectionName != null)
                    {
                        entrys[currentSectionName] = currentSection;
                    }

                    currentSectionName = line.Trim().Trim('[', ']');
                    currentSection = new GroupDictionary<string, string>();
                    continue;
                }

                string[] content = line.Split(KeyValueChar, 2, StringSplitOptions.RemoveEmptyEntries);
                if (content.Length <= 1)
                    continue;
                currentSection[content[0]].Add(content[1]);
            }

            if (currentSectionName != null)
                entrys[currentSectionName] = currentSection;

            var sections = new Dictionary<string, IniSection>(entrys.Count);

            foreach (var entry in entrys)
            {
                var entries = new Dictionary<string, IniEntry>(entry.Value.Count);

                foreach (var keyEntry in entry.Value)
                {
                    if (keyEntry.Value.Count < 1)
                        entries[keyEntry.Key] = new ListIniEntry(keyEntry.Key, new List<string>(keyEntry.Value));
                    else
                        entries[keyEntry.Key] = new SingleIniEntry(keyEntry.Key, keyEntry.Value.ElementAt(0));
                }

                sections[entry.Key] = new IniSection(entries, entry.Key);
            }

            return new IniFile(sections);
        }
    }
}