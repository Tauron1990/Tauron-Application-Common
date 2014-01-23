using System.Collections.Generic;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Modules
{
    [PublicAPI]
    public static class AddInListner
    {
        [NotNull]
        public static List<AddIn> AddIns { get; private set; }

        static AddInListner()
        {
            AddIns = new List<AddIn>();
        }
    }
}
