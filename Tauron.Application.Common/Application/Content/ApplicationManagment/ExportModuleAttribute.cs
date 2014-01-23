using System;
using Tauron.Application.Ioc;

namespace Tauron.Application
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class ExportModuleAttribute : ExportAttribute
    {
        public ExportModuleAttribute() : base(typeof(IModule))
        {
        }
    }
}