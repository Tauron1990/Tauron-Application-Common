using System;
using System.Reflection;
using ExpressionBuilder;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;


namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    public class EventMemberInjector : MemberInjector
    {
        public EventMemberInjector([NotNull] ImportMetadata metadata, [NotNull] IEventManager manager, [NotNull] MemberInfo member)
        {
            _metadata = Argument.NotNull(metadata, nameof(metadata));
            _manager = Argument.NotNull(manager, nameof(manager));
            _member = Argument.NotNull(member, nameof(member));
        }
        
        public override void Inject(CompilationUnit target, IContainer container, ImportMetadata metadata, IImportInterceptor interceptor, ErrorTracer errorTracer,
            BuildParameter[] parameters)
        {
            errorTracer.Phase = "EventManager Inject " + metadata.ContractName;

            try
            {
                var eventInfo = _member as EventInfo;
                if (eventInfo != null)
                {
                    target.AddCode(Operation.Invoke(Operation.Constant(_manager), nameof(_manager.AddPublisher), Operation.Constant(_metadata.ContractName),
                        Operation.Constant(eventInfo), Operation.Variable(CompilationUnit.TargetName), Operation.Null()));
                }

                var method = _member as MethodInfo;
                if (method != null)
                {
                    target.AddCode(Operation.Invoke(Operation.Constant(_manager), nameof(_manager.AddEventHandler), Operation.Constant(_metadata.ContractName),
                        Operation.Constant(method), Operation.Variable(CompilationUnit.TargetName), Operation.Null()));
                }
                
            }
            catch (Exception e)
            {
                errorTracer.Exceptional = true;
                errorTracer.Exception = e;
            }
        }
        
        private readonly IEventManager _manager;
        
        private readonly MemberInfo _member;
        
        private readonly ImportMetadata _metadata;
    }
}