// The file ListIniEntry.cs is part of Tauron.Application.Files.
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
    [Serializable]
    public sealed class ListIniEntry : IniEntry
    {
        internal ListIniEntry([NotNull] string key, [NotNull] List<string> values)
            : base(key)

        {
            Values = values;
        }

        internal ListIniEntry([NotNull] SingleIniEntry entry)
            : base(entry.Key)
        {
            Values = new List<string>(1) {entry.Value};
        }

        [NotNull]
        public List<string> Values { get; private set; }
    }
}