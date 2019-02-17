using System;
using Tauron.Application.Ioc;

namespace Tauron.Application.Common.MVVM.Dynamic
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class CreateRuleCallAttribute : ExportMetadataBaseAttribute
    {
        public CreateRuleCallAttribute() 
            : base(MvvmDynamicExtension.MvvmDynamicMeta, true) { }
    }
}