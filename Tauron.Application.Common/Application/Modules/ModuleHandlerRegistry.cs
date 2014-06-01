using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Modules
{
    [PublicAPI]
    public static class ModuleHandlerRegistry
    {
        private static readonly GroupDictionary<Type, Action<MemberInfo, Attribute, IModule>> Handlers = Initialize();

        private static GroupDictionary<Type, Action<MemberInfo, Attribute, IModule>> Initialize()
        {
            var temp = new GroupDictionary<Type, Action<MemberInfo, Attribute, IModule>>();

            temp[typeof (AddinDescriptionAttribute)].Add(AddInListner.OnProgress);

            return temp;
        }

        public static void RegisterHandler<TAttribute>([NotNull] Action<MemberInfo, Attribute, IModule> handler)
            where TAttribute : Attribute
        {
            Contract.Requires<ArgumentNullException>(handler != null, "handler");

            Handlers[typeof (TAttribute)].Add(handler);
        }

        [NotNull]
        public static IEnumerable<Action<MemberInfo, Attribute, IModule>> GetHandler([NotNull] Type key)
        {
            Contract.Requires<ArgumentNullException>(key != null, "key");
            Contract.Ensures(Contract.Result<IEnumerable<Action<MemberInfo, Attribute, IModule>>>() != null);

            ICollection<Action<MemberInfo, Attribute, IModule>> action;
            return Handlers.TryGetValue(key, out action) ? action : Enumerable.Empty<Action<MemberInfo, Attribute, IModule>>();
        }

        public static void Progress([NotNull] IModule module)
        {
            Contract.Requires<ArgumentNullException>(module != null, "module");

            Type type = module.GetType();

            foreach (var info in type.GetMembers(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                foreach (var attribute in info.GetCustomAttributes(true))
                {
                    foreach (var action in GetHandler(attribute.GetType()))
                    {
                        action(info, (Attribute)attribute, module);
                    }
                }
            }
        }
    }
}
