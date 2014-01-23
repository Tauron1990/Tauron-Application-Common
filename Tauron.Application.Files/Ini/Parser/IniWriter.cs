// The file IniWriter.cs is part of Tauron.Application.Files.
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

using System.Globalization;
using System.IO;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Files.Ini.Parser
{
    [PublicAPI]
    public class IniWriter
    {
        private readonly IniFile _file;
        private readonly TextWriter _writer;

        public IniWriter(IniFile file, TextWriter writer)
        {
            _file = file;
            _writer = writer;
        }

        public void Write()
        {
            try
            {
                foreach (var section in _file.Sections)
                {
                    _writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "[{0}]", section.Name));
                    foreach (var iniEntry in section.Entries)
                    {
                        var entry = iniEntry as SingleIniEntry;
                        if (entry != null)
                            _writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}={1}", entry.Key, entry.Value));
                        else
                        {
                            var entry2 = (ListIniEntry) iniEntry;
                            string name = entry2.Key;
                            foreach (var value in entry2.Values)
                            {
                                _writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}={1}", name, value));
                            }
                        }
                    }
                }
            }
            finally
            {
                _writer.Flush();
                _writer.Dispose();
            }
        }
    }
}