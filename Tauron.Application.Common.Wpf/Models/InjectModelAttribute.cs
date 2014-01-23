using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Tauron.Application.Ioc;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Models
{
    [AttributeUsage(
        AttributeTargets.Constructor | AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Parameter
        | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class InjectModelAttribute : InjectAttribute
    {
        public InjectModelAttribute([NotNull] string name)
            : base(typeof(IModel))
        {
            Contract.Requires<ArgumentNullException>(name != null, "name");
            ContractName = name;
        }

        public bool EnablePropertyInheritance { get; set; }

        public override Dictionary<string, object> CreateMetadata()
        {
            return new Dictionary<string, object>
            {
                {PropertyModelExtension.EnablePropertyInheritanceMetadataName, EnablePropertyInheritance}
            };
        }
    }
}
