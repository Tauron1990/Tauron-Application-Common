using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Tauron.Application.Ioc
{
    [AttributeUsage(
        AttributeTargets.Constructor | AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Parameter
        | AttributeTargets.Property)]
    [PublicAPI]
    [MeansImplicitUse(ImplicitUseKindFlags.Assign)]
    public class ImportAttribute : Attribute
    {
        public ImportAttribute()
        {
            ContractName = null;
            Optional = false;
        }

        public ImportAttribute([NotNull] string contractName)
            : this() => ContractName = contractName;

        public ImportAttribute([NotNull] Type @interface)
            : this() => Interface = @interface;

        [CanBeNull]
        public string ContractName { get; set; }

        [CanBeNull]
        public Type Interface { get; private set; }

        public bool Optional { get; set; }


        [NotNull]
        public virtual Dictionary<string, object> CreateMetadata() => new Dictionary<string, object>();
    }
}