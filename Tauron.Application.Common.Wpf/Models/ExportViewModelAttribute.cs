using System;
using System.Diagnostics.Contracts;
using Tauron.Application.Ioc;
using Tauron.Application.Ioc.LifeTime;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Models
{
    [PublicAPI, BaseTypeRequired(typeof(ViewModelBase))]
    public class ExportViewModelAttribute : ExportAttribute, INameExportMetadata
    {
        public ExportViewModelAttribute([NotNull] string name) : base(typeof(ViewModelBase))
        {
            Contract.Requires<ArgumentNullException>(name != null, "name");

            ContractName = name;
            IgnoreIntercepion = true;
        }

        public string Name { get { return ContractName; } }

        public bool IgnoreIntercepion { get; set; }

        protected override bool HasMetadata
        {
            get { return true; }
        }

        public override string DebugName
        {
            get { return Name; }
        }

        protected override LifetimeContextAttribute OverrideDefaultPolicy
        {
            get { return new NotSharedAttribute(); }
        }
    }
}
