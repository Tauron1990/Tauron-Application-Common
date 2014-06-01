using System;
using System.Diagnostics.Contracts;
using System.Windows.Controls;
using Tauron.Application.Ioc;
using Tauron.Application.Ioc.LifeTime;
using Tauron.Application.Views.Core;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Views
{
    [PublicAPI, BaseTypeRequired(typeof(Control))]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ExportViewAttribute : ExportAttribute, ISortableViewExportMetadata
    {
        public ExportViewAttribute([NotNull] string viewName) : base(typeof (Control))
        {
            Contract.Requires<ArgumentNullException>(viewName != null, "viewName");

            Name = viewName;
            Order = int.MaxValue;
        }

        public string Name { get; private set; }

        protected override LifetimeContextAttribute OverrideDefaultPolicy
        {
            get { return new NotSharedAttribute(); }
        }

        public override string DebugName
        {
            get { return Name; }
        }

        protected override bool HasMetadata
        {
            get { return true; }
        }

        public int Order { get; set; }
    }
}