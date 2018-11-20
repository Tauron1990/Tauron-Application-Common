using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;

namespace Tauron.Application.Modules
{
    [PublicAPI]
    public static class AddInListner
    {
        static AddInListner() => AddIns = new List<AddinDescription>();

        [NotNull]
        public static List<AddinDescription> AddIns { get; private set; }

        internal static void OnProgress([NotNull] MemberInfo member, [NotNull] Attribute attr, [NotNull] IModule module)
        {
            Argument.NotNull(member, nameof(member));
            Argument.NotNull(attr, nameof(attr));
            Argument.NotNull(module, nameof(module));

            var desc = member.GetInvokeMember<AddinDescription>(module);

            if (desc != null)
                AddIns.Add(desc);
        }
    }
}