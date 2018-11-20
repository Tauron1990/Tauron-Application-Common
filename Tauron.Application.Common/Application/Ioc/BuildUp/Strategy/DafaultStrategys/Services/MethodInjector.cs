using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    [PublicAPI]
    public class ParameterMemberInfo : MemberInfo
    {
        public ParameterMemberInfo([NotNull] ParameterInfo parameterInfo) => ParameterInfo = Argument.NotNull(parameterInfo, nameof(parameterInfo));

        [NotNull]
        public ParameterInfo ParameterInfo { get; private set; }

        public override MemberTypes MemberType => 0;

        public override string Name => ParameterInfo.Name;

        public override Type DeclaringType => ParameterInfo.Member.DeclaringType;

        public override Type ReflectedType => ParameterInfo.Member.ReflectedType;

        public override int MetadataToken => ParameterInfo.MetadataToken;

        public override Module Module => ParameterInfo.Member.Module;

        public override object[] GetCustomAttributes(bool inherit) => ParameterInfo.GetCustomAttributes(inherit);

        public override bool IsDefined(Type attributeType, bool inherit) => ParameterInfo.IsDefined(attributeType, inherit);

        public override object[] GetCustomAttributes(Type attributeType, bool inherit) => ParameterInfo.GetCustomAttributes(attributeType, inherit);

        [NotNull]
        public override IList<CustomAttributeData> GetCustomAttributesData() => ParameterInfo.GetCustomAttributesData();
    }
    
    public class MethodInjector : MemberInjector
    {
        public MethodInjector([NotNull] MethodInfo method, [NotNull] IMetadataFactory metadataFactory, [NotNull] IEventManager eventManager,
            [NotNull] [ItemNotNull] IResolverExtension[] resolverExtensions)
        {
            _method = Argument.NotNull(method, nameof(method));
            _metadataFactory = Argument.NotNull(metadataFactory, nameof(metadataFactory));
            _eventManager = Argument.NotNull(eventManager, nameof(eventManager));
            _resolverExtensions = Argument.NotNull(resolverExtensions, nameof(resolverExtensions));
        }
        
        public override void Inject(object target, IContainer container, ImportMetadata metadata, IImportInterceptor interceptor, ErrorTracer errorTracer,
            BuildParameter[] parameters)
        {
            if (metadata.Metadata != null)
            {
                if (metadata.Metadata.TryGetValue(AopConstants.EventMetodMetadataName, out var obj))
                {
                    if ((bool) obj)
                    {
                        var topic = (string) metadata.Metadata[AopConstants.EventTopicMetadataName];
                        _eventManager.AddEventHandler(topic, _method, target, errorTracer);
                        return;
                    }
                }
            }

            var parms = _method.GetParameters();
            var args = new List<object>();

            foreach (var parameterInfo in parms.Select(p => new ParameterMemberInfo(p)))
                new ParameterHelper(_metadataFactory, parameterInfo, args, _resolverExtensions).Inject(target, container, metadata, interceptor, errorTracer, parameters);

            _method.Invoke(target, args.ToArray());
        }
        
        private class ParameterHelper : Injectorbase<ParameterMemberInfo>
        {
            private readonly List<object> _parameters;
            
            public ParameterHelper([NotNull] IMetadataFactory metadataFactory, [NotNull] ParameterMemberInfo parameter,
                [NotNull] List<object> parameters, [NotNull] IResolverExtension[] resolverExtensions)
                : base(metadataFactory, parameter, resolverExtensions) => _parameters = Argument.NotNull(parameters, nameof(parameters));

            protected override Type MemberType => Member.ParameterInfo.ParameterType;
            
            protected override void Inject(object target, object value) => _parameters.Add(value);
        }
        
        private readonly IEventManager _eventManager;

        private readonly IResolverExtension[] _resolverExtensions;
        
        private readonly IMetadataFactory _metadataFactory;
        
        private readonly MethodInfo _method;

    }
}