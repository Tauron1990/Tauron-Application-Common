using System;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;


namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    public class EventMemberInjector : MemberInjector
    {
        private readonly IEventManager _manager;
        private readonly MemberInfo _member;
        private readonly ImportMetadata _metadata;
        private readonly CompilationUnit _unit;

        public EventMemberInjector([NotNull] ImportMetadata metadata, [NotNull] IEventManager manager, [NotNull] MemberInfo member, CompilationUnit unit)
        {
            _unit = unit;
            _metadata = Argument.NotNull(metadata, nameof(metadata));
            _manager = Argument.NotNull(manager, nameof(manager));
            _member = Argument.NotNull(member, nameof(member));
        }
        
        public override void Inject(CompilationUnit target, BuildEngine container, ImportMetadata metadata, IImportInterceptor interceptor, ErrorTracer errorTracer, BuildParameter[] parameters,
            CompilationUnit unit)
        {
            errorTracer.Phase = "EventManager Inject " + metadata.ContractName;

            try
            {
                var eventInfo = _member as EventInfo;
                if (eventInfo != null)
                {
                    target.Expressions.Add(
                        Expression.Call());

                    target.AddCode(Operation.Invoke(Operation.Constant(_manager), nameof(_manager.AddPublisher), Operation.Constant(_metadata.ContractName),
                        Operation.Constant(eventInfo), Operation.Variable(_unit.TargetName), Operation.Null()));
                }

                var method = _member as MethodInfo;
                if (method != null)
                {
                    target.AddCode(Operation.Invoke(Operation.Constant(_manager), nameof(_manager.AddEventHandler), Operation.Constant(_metadata.ContractName),
                        Operation.Constant(method), Operation.Cast(Operation.Variable(_unit.TargetName), typeof(Delegate)), Operation.Null()));
                }
                
            }
            catch (Exception e)
            {
                errorTracer.Exceptional = true;
                errorTracer.Exception = e;
            }
        }
    }
}