using System;
using Tauron.Application.Ioc;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Shell
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    [PublicAPI, BaseTypeRequired(typeof(IService))]
    public sealed class ExportServiceAttribute : ExportAttribute
    {
        public ExportServiceAttribute([NotNull] string name) 
            : base(typeof(IService))
        {
            if (name == null) throw new ArgumentNullException("name");

            ContractName = name;
        }
    }
}