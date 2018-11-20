using System;
using System.Reflection;
using Castle.DynamicProxy;
using JetBrains.Annotations;
using NLog;
using Tauron.Application.Ioc;
using Tauron.Application.Ioc.LifeTime;


namespace Tauron.Application.Common.CastleProxy.Aop
{
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class PropertyAspectAttrebute : AspectBaseAttribute
    {
        private PropertyInfo _propertyInfo;

        protected override void Intercept(IInvocation invocation, ObjectContext context)
        {
            var name = invocation.Method.Name;
            var getter = name.StartsWith(AopConstants.PropertyGetter, StringComparison.Ordinal);
            if (_propertyInfo == null)
            {
                name = name.Remove(0, getter ? AopConstants.PropertyGetter.Length : AopConstants.PropertySetter.Length);

                _propertyInfo = invocation.Method.DeclaringType?.GetProperty(name, AopConstants.DefaultBindingFlags);
            }

            if (_propertyInfo == null)
            {
                LogManager.GetCurrentClassLogger().Log(LogLevel.Warn, $"No Property Found {invocation.Method} -- {invocation.Method.DeclaringType}");
                invocation.Proceed();
                return;
            }

            if (getter) OnGet(invocation, context, _propertyInfo);
            else OnSet(invocation, context, _propertyInfo);
        }

        protected virtual void OnGet([NotNull] IInvocation invocation, [NotNull] ObjectContext context, [NotNull] PropertyInfo propertyInfo) => invocation.Proceed();

        protected virtual void OnSet([NotNull] IInvocation invocation, [NotNull] ObjectContext context, [NotNull] PropertyInfo propertyInfo) => invocation.Proceed();
    }
}