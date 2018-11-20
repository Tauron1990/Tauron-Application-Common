using System;
using System.Reflection;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using JetBrains.Annotations;
using Tauron.Application.Ioc.LifeTime;

namespace Tauron.Application.Common.CastleProxy.Aop.Threading
{
    public enum TaskOption
    {
        Worker,
        Task,
        UIThread
    }
    
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ScheduleAttribute : AspectBaseAttribute
    {
        private bool _isOk;
        
        public ScheduleAttribute()
        {
            Order = 0;
            CreationOptions = TaskCreationOptions.None;
            TaskOption = TaskOption.Worker;
        }
        [NotNull]
        public override IInterceptor Create([NotNull] MemberInfo info)
        {
            _isOk = ((MethodInfo) info).ReturnType == typeof(void);

            CommonConstants.LogCommon(false,
                "AOP Module: The member {0}.{1} has no Void Return",
                info.DeclaringType?.FullName,
                info.Name);

            return base.Create(info);
        }
        
        protected override void Intercept(IInvocation invocation, ObjectContext context)
        {
            if (!_isOk)
            {
                invocation.Proceed();
                return;
            }

            switch (TaskOption)
            {
                case TaskOption.Worker:
                    CommonApplication.QueueWorkitemAsync(invocation.Proceed, false);
                    break;
                case TaskOption.Task:
                    Task.Factory.StartNew(invocation.Proceed, CreationOptions);
                    break;
                case TaskOption.UIThread:
                    CommonApplication.QueueWorkitemAsync(invocation.Proceed, true);
                    break;
                default:
                    CommonConstants.LogCommon(false, "Invalid Schedule TaskOption: {0}.{1}", invocation.TargetType, invocation.Method);
                    invocation.Proceed();
                    break;
            }
        }
        
        public TaskCreationOptions CreationOptions { get; set; }

        public TaskOption TaskOption { get; set; }
    }
}