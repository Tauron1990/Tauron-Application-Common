using System;
using System.Reflection;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;


namespace Tauron.Application.Ioc.BuildUp.Strategy.DefaultStrategy
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
            BuildParameter[] parameters, FactoryCacheEntry entry)
        {
            errorTracer.Phase = "EventManager Inject " + metadata.ContractName;

            try
            {
                var meta = _metadata;
                var manager = _manager;
                switch (_member)
                {
                    case EventInfo eventInfo:
                        manager.AddPublisher(meta.ContractName, eventInfo, target, errorTracer);
                        entry.BuildActions.Add(c => manager.AddPublisher(meta.ContractName, eventInfo, c.Target, c.ErrorTracer));
                        break;
                    case MethodInfo method:
                        manager.AddEventHandler(meta.ContractName, method, target, errorTracer);
                        entry.BuildActions.Add(c => manager.AddEventHandler(meta.ContractName, method, c.Target, c.ErrorTracer));
                        break;
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