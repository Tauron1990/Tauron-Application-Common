using System;
using System.Reflection;
using JetBrains.Annotations;
using Tauron.Application.Common.CastleProxy.Impl.LifeTime;
using Tauron.Application.Ioc.LifeTime;

namespace Tauron.Application.Common.CastleProxy
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public abstract class ObjectContextPropertyAttribute : Attribute
    {
        protected internal abstract void Register([NotNull] ObjectContext context, [NotNull] MemberInfo info, [NotNull] object target);
    }
}