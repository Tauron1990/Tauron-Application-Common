using System;
using System.Reflection;
using Castle.DynamicProxy;
using JetBrains.Annotations;
using NLog;
using Tauron.Application.Ioc;
using Tauron.Application.Ioc.LifeTime;

namespace Tauron.Application.Common.CastleProxy.Aop
{
    [AttributeUsage(AttributeTargets.Event)]
    [PublicAPI]
    public abstract class EventAspectAttribute : AspectBaseAttribute
    {
        private EventInfo _eventInfo;

        protected override void Intercept(IInvocation invocation, ObjectContext context)
        {
            var name = invocation.Method.Name;
            var getter = name.StartsWith(AopConstants.EventAdder, StringComparison.Ordinal);
            if (_eventInfo == null)
            {
                name = name.Remove(0, getter ? AopConstants.EventAdder.Length : AopConstants.EventRemover.Length);

                _eventInfo = invocation.Method.DeclaringType?.GetEvent(name, AopConstants.DefaultBindingFlags);
            }

            if (_eventInfo == null)
            {
                LogManager.GetCurrentClassLogger().Log(LogLevel.Warn, $"No Event Found {invocation.Method} -- {invocation.Method.DeclaringType}");
                invocation.Proceed();
                return;
            }

            if (getter) OnGet(invocation, context, _eventInfo);
            else OnSet(invocation, context, _eventInfo);
        }

        protected virtual void OnGet([NotNull] IInvocation invocation, [NotNull] ObjectContext context, [NotNull] EventInfo eventInfo)
        {
            invocation.Proceed();
        }

        protected virtual void OnSet([NotNull] IInvocation invocation, [NotNull] ObjectContext context, [NotNull] EventInfo eventInfo)
        {
            invocation.Proceed();
        }
    }
}