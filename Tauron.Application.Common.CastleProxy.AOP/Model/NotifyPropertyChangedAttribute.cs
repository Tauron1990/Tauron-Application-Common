using System;
using System.ComponentModel;
using System.Linq;
using Castle.DynamicProxy;
using JetBrains.Annotations;
using Tauron.Application.Ioc;
using Tauron.Application.Ioc.LifeTime;

namespace Tauron.Application.Common.CastleProxy.Aop.Model
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class NotifyPropertyChangedAttribute : AspectBaseAttribute
    {
        private Action<string> _eventInvoker;

        public NotifyPropertyChangedAttribute()
        {
            AlternativePropertyChangedName = "OnPropertyChanged";
            Order = 900;
        }

        [NotNull]
        public string AlternativePropertyChangedName { get; set; }

        protected override void Initialize(object target, ObjectContext context, string contextName)
        {
            if (target is INotifyPropertyChangedMethod metod)
                _eventInvoker = metod.OnPropertyChanged;
            else
            {
                var info =
                    target.GetType()
                        .GetMethods(AopConstants.DefaultBindingFlags)
                        .FirstOrDefault(
                            metodInfo =>
                                metodInfo.Name == AlternativePropertyChangedName
                                && metodInfo.ReturnType == typeof(void));
                if (info != null && info.GetParameters().Length == 1)
                {
                    var parameterType = info.GetParameters()[0].ParameterType;
                    if (parameterType == typeof(PropertyChangedEventArgs)) _eventInvoker = s => info.Invoke(target, new PropertyChangedEventArgs(s));
                    else if (parameterType == typeof(string)) _eventInvoker = s => info.Invoke(target, s);
                    else
                    {
                        CommonConstants.LogCommon(false, "AOP Module: No PropertyChanged Method Found: Class:{0} AltName:{1}",
                            target.GetType(),
                            AlternativePropertyChangedName);
                    }
                }
            }

            base.Initialize(target, context, contextName);
        }

        protected override void Intercept(IInvocation invocation, ObjectContext context)
        {
            invocation.Proceed();

            if (_eventInvoker == null) return;

            if (invocation.Method.Name.StartsWith(AopConstants.PropertySetter, StringComparison.Ordinal)
                && invocation.Method.IsSpecialName) _eventInvoker(invocation.Method.Name.Remove(0, AopConstants.PropertySetter.Length));
        }
    }
}