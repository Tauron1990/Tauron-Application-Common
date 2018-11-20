using System;
using System.Reflection;
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
        
        public override void Inject(object target, IContainer container, ImportMetadata metadata, IImportInterceptor interceptor, ErrorTracer errorTracer,
            BuildParameter[] parameters)
        {
            errorTracer.Phase = "EventManager Inject " + metadata.ContractName;

            try
            {
                var eventInfo = _member as EventInfo;
                if (eventInfo != null) _manager.AddPublisher(_metadata.ContractName, eventInfo, target, errorTracer);

                var method = _member as MethodInfo;
                if (method != null) _manager.AddEventHandler(_metadata.ContractName, method, target, errorTracer);
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