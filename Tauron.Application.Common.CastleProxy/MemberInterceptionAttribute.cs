using System;
using System.Reflection;
using Castle.DynamicProxy;
using JetBrains.Annotations;
using Tauron.Application.Ioc.LifeTime;

namespace Tauron.Application.Common.CastleProxy
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event | AttributeTargets.Class,AllowMultiple = true)]
    [PublicAPI]
    public abstract class MemberInterceptionAttribute : Attribute
    {
        public abstract IInterceptor Create(MemberInfo info);
        
        protected internal abstract void Initialize(object target, ObjectContext context, string contextName);
    }
}