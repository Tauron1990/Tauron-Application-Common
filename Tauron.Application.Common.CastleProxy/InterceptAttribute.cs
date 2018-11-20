using System;
using Castle.DynamicProxy;
using JetBrains.Annotations;
using Tauron.Application.Ioc;

namespace Tauron.Application.Common.CastleProxy
{
    [AttributeUsage(AttributeTargets.Class)]
    [PublicAPI]
    public class InterceptAttribute : ExportMetadataBaseAttribute
    {
        public InterceptAttribute()
            : base(AopConstants.InterceptMetadataName, null) => InternalValue = this;

        [CanBeNull]
        public virtual IInterceptor Create() => null;

        public virtual void Initialize([NotNull] object target) { }
    }
}