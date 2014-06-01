using System;
using System.Diagnostics.Contracts;
using System.Windows;
using Tauron.Application.Ioc;
using Tauron.Application.Ioc.LifeTime;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Views
{
    [PublicAPI, BaseTypeRequired(typeof(Window))]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ExportWindowAttribute: ExportAttribute, INameExportMetadata
    {
        public ExportWindowAttribute([NotNull] string viewName)
            : base(typeof(Window))
        {
            Contract.Requires<ArgumentNullException>(viewName != null, "viewName");

            Name = viewName;
        }

        public string Name { get; private set; }

        public override string DebugName
        {
            get { return Name; }
        }

        protected override LifetimeContextAttribute OverrideDefaultPolicy
        {
            get { return new NotSharedAttribute(); }
        }

        protected override bool HasMetadata
        {
            get { return true; }
        }
    }
}
