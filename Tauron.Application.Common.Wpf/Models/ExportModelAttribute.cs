using System;
using System.Diagnostics.Contracts;
using Tauron.Application.Ioc;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Models
{
    [PublicAPI ,BaseTypeRequired(typeof(IModel))]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class ExportModelAttribute : ExportAttribute, INameExportMetadata
    {
        public ExportModelAttribute([NotNull] string name)
            : base(typeof(IModel))
        {
            Contract.Requires<ArgumentNullException>(name != null, "name");

            ContractName = name;
        }

        public string Name { get { return ContractName; } }

        public override string DebugName
        {
            get { return Name; }
        }

        protected override bool HasMetadata
        {
            get { return true; }
        }
    }
}
