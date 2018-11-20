using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Tauron.Application.Ioc.LifeTime;

namespace Tauron.Application.Ioc
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    [MeansImplicitUse(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public class ExportAttribute : Attribute
    {
        public ExportAttribute([NotNull] Type export)
        {
            Argument.NotNull(export, nameof(export));
            Export = export;
        }

        [CanBeNull]
        public string ContractName { get; set; }

        [NotNull]
        public Type Export { get; }

        [CanBeNull]
        [UsedImplicitly]
        public virtual string DebugName => ContractName;

        [NotNull]
        public IEnumerable<Tuple<string, object>> Metadata
        {
            get
            {
                if (!HasMetadata) yield break;

                foreach (var property in
                    GetType().GetProperties().Where(property => property.Name != "Metadata"))
                    yield return Tuple.Create(property.Name, property.GetValue(this));
            }
        }

        protected virtual bool HasMetadata => false;

        [CanBeNull]
        protected virtual LifetimeContextAttribute OverrideDefaultPolicy => null;

        [CanBeNull]
        internal LifetimeContextAttribute GetOverrideDefaultPolicy() => OverrideDefaultPolicy;
    }
}