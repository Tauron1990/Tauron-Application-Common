using System;
using Tauron.Application.Ioc;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Shell
{
    [AttributeUsage(
        AttributeTargets.Constructor | AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Parameter
        | AttributeTargets.Property, AllowMultiple = false)]
    [PublicAPI]
    public class InjectServiceAttribute : InjectAttribute
    {
        public InjectServiceAttribute([NotNull] string name)
            : base(typeof(IService))
        {
            if (name == null) throw new ArgumentNullException("name");

            ContractName = name;
        }
    }
}