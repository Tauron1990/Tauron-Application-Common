using System;
using JetBrains.Annotations;

namespace Tauron.Application.Common.CastleProxy.Aop.Threading
{
    [PublicAPI]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public abstract class ContextPropertyAttributeBase : ObjectContextPropertyAttribute
    {
        protected ContextPropertyAttributeBase() => HolderName = string.Empty;

        [NotNull]
        public string HolderName { get; set; }

    }
}