using System;
using System.Diagnostics.Contracts;
using System.Windows.Controls;
using Tauron.Application.Ioc;
using Tauron.Application.Ioc.LifeTime;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Views
{
    [PublicAPI, BaseTypeRequired(typeof(Control))]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ExportViewAttribute : ExportAttribute, INameExportMetadata
    {
        public ExportViewAttribute([NotNull] string viewName) : base(typeof (Control))
        {
            Contract.Requires<ArgumentNullException>(viewName != null, "viewName");

            Name = viewName;
        }

        public string Name { get; private set; }

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