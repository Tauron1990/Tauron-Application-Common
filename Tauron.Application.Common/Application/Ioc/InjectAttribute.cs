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
    public class InjectAttribute : Attribute
    {
        public InjectAttribute()
        {
            ContractName = null;
            Optional = false;
        }

        public InjectAttribute(string contractName)
            : this() => ContractName = contractName;

        public InjectAttribute(Type @interface)
            : this() => Interface = @interface;

        public string ContractName { get; set; }

        public Type Interface { get; private set; }

        public bool Optional { get; set; }

        [NotNull]
        public virtual Dictionary<string, object> CreateMetadata() => new Dictionary<string, object>();
    }
}