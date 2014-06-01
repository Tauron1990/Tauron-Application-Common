using System;
using System.Collections.Generic;
using System.Reflection;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Modules
{
    [PublicAPI]
    public static class AddInListner
    {
        [NotNull]
        public static List<AddinDescription> AddIns { get; private set; }

        static AddInListner()
        {
            AddIns = new List<AddinDescription>();
        }

        internal static void OnProgress([NotNull] MemberInfo member, [NotNull] Attribute attr, [NotNull] IModule module)
        {
            var desc = member.GetInvokeMember<AddinDescription>(module);

            if(desc != null)
                AddIns.Add(desc);
        }
    }
}
