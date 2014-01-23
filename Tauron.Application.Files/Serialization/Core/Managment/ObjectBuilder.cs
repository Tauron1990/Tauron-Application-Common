// The file ObjectBuilder.cs is part of Tauron.Application.Files.
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
using System.Linq;
using System.Reflection;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Files.Serialization.Core.Managment
{
    public class ObjectBuilder
    {
        public ObjectBuilder([CanBeNull] Type target)
        {
            if(target == null) return;

            int count = -1;
            ConstructorInfo info = null;

            foreach (var con in target.GetConstructors())
            {
                count = con.GetParameters().Count();
                switch (count)
                {
                    case 0:
                        info = con;
                        break;
                    case 1:
                        info = con;
                        break;
                }
            }

            if (info == null) return;

            SetConstructor(info, count);
        }

        [CanBeNull]
        public object CustomObject { get; set; }

        [CanBeNull]
        public Func<object, object> BuilderFunc { get; set; }

        public void SetConstructor([CanBeNull] ConstructorInfo info, int parmCount)
        {
            BuilderFunc = null;

            if (info == null) return;

            bool single = parmCount == 0;

            if (parmCount == -1)
            {
                switch (info.GetParameters().Count())
                {
                    case 0:
                        single = true;
                        break;
                    case 1:
                        single = false;
                        break;
                }
            }

            if (parmCount > 1) return;

            if (single) BuilderFunc = o => info.Invoke(null);
            else BuilderFunc = o => info.Invoke(new[] {o});
        }

        [CanBeNull]
        public Exception Verfiy()
        {
            return BuilderFunc == null ? new SerializerElementNullException("BuilderFunc") : null;
        }
    }
}