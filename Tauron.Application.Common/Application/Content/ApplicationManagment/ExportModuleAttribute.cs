using System;
using Tauron.Application.Ioc;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false), BaseTypeRequired(typeof (IModule))]
    public class ExportModuleAttribute : ExportAttribute
    {
        public ExportModuleAttribute() : base(typeof(IModule))
        {
        }
    }
}