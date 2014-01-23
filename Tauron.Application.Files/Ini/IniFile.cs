// The file IniFile.cs is part of Tauron.Application.Files.
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
using Tauron.Application.Files.Ini.Parser;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Files.Ini
{
    [PublicAPI, Serializable]
    public class IniFile
    {
        #region Content Load

        public static IniFile Parse(TextReader reader)
        {
            using (reader)
                return new IniParser(reader).Parse();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Objekte verwerfen, bevor Bereich verloren geht")]
        public static IniFile ParseContent(string content)
        {
            return Parse(new StringReader(content));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Objekte verwerfen, bevor Bereich verloren geht")]
        public static IniFile ParseFile(string path)
        {
            return Parse(new StreamReader(path));
        }

        public static IniFile ParseStream(Stream stream)
        {
            return Parse(new StreamReader(stream));
        }

        #endregion

        private readonly Dictionary<string, IniSection> _sections;

        public IniFile(Dictionary<string, IniSection> sections)
        {
            _sections = sections;
        }

        public IniFile()
        {
            _sections = new Dictionary<string, IniSection>();
        }

        public ReadOnlyEnumerator<IniSection> Sections
        {
            get { return new ReadOnlyEnumerator<IniSection>(_sections.Values); }
        }

        public IniSection this[string name]
        {
            get
            {
                IniSection section;
                return _sections.TryGetValue(name, out section) ? section : null;
            }
        }

        public IniSection AddSection(string name)
        {
            var section = new IniSection(name);
            _sections[name] = section;
            return section;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Objekte verwerfen, bevor Bereich verloren geht")]
        public void Save(string path)
        {
            new IniWriter(this, new StreamWriter(path)).Write();
        }


        public string GetData(string name, string sectionName, string defaultValue)
        {
            SingleIniEntry keyData = GetSection(sectionName).GetData(name);
            if (String.IsNullOrWhiteSpace(keyData.Value))
                keyData.Value = defaultValue;

            return keyData.Value;
        }


        public IniSection GetSection(string name)
        {
            IniSection data = this[name];

            if (data != null) return data;

            AddSection(name);
            data = this[name];

            return data;
        }

        public void SetData(string sectionName, string name, string value)
        {
            GetSection(sectionName).GetData(name).Value = value;
        }
    }
}