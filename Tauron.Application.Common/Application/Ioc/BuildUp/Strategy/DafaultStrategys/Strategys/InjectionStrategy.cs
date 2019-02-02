using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExpressionBuilder;
using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.Application.Ioc.Components;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    public class InjectionStrategy : StrategyBase
    {
        private IEventManager _eventManager;
        private IMetadataFactory _factory;
        private IImportInterceptorFactory[] _interceptorFactories;

        public override void Initialize(ComponentRegistry components)
        {
            _eventManager = components.Get<IEventManager>();
            _factory = components.Get<IMetadataFactory>();
            _interceptorFactories = components.GetAll<IImportInterceptorFactory>().ToArray();
        }

        public override void OnBuild(IBuildContext context)
        {
            context.ErrorTracer.Phase = "Injecting Imports for " + context.Metadata;
            
            foreach (var policy in context.Policys.GetAll<InjectMemberPolicy>())
            {
                policy.Injector.Inject(context.CompilationUnit, context.Container, policy.Metadata,
                    policy.Interceptors == null ? null : new CompositeInterceptor(policy.Interceptors),
                    context.ErrorTracer, context.Parameters);

                if (context.ErrorTracer.Exceptional) return;
            }

            context.CompilationUnit.AddAndPush(CodeLine.Return());
        }

        public override void OnPerpare(IBuildContext context)
        {
            if (!context.CanUseBuildUp()) return;

            context.ErrorTracer.Phase = "Loading Injections for " + context.Metadata;

            var members = context.ExportType.GetMembers(AopConstants.DefaultBindingFlags);

            List<IImportInterceptor> importInterceptors = null;
            var intpol = context.Policys.Get<ExternalImportInterceptorPolicy>();
            if (intpol != null) importInterceptors = intpol.Interceptors;

            foreach (
                var temp in
                _interceptorFactories.Select(
                        importInterceptorFactory => importInterceptorFactory.CreateInterceptor(context.Metadata))
                    .Where(temp => temp != null))
                if (importInterceptors == null)
                    importInterceptors = new List<IImportInterceptor> {temp};
                else
                    importInterceptors.Add(temp);

            foreach (var importMetadata in context.Metadata.Export.ImportMetadata)
            {
                var info = members.FirstOrDefault(inf => inf.Name == importMetadata.MemberName);
                if (info == null) continue;

                MemberInjector injector;
                switch (info.MemberType)
                {
                    case MemberTypes.Event:
                        injector = new EventMemberInjector(importMetadata, _eventManager, info);
                        break;
                    case MemberTypes.Field:
                        injector = new FieldInjector(_factory, (FieldInfo) info, context.ResolverExtensions);
                        break;
                    case MemberTypes.Property:
                        injector = new PropertyInjector(_factory, (PropertyInfo) info, context.ResolverExtensions);
                        break;
                    case MemberTypes.Method:
                        injector = new MethodInjector((MethodInfo) info, _factory, _eventManager, context.ResolverExtensions);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                context.Policys.Add(new InjectMemberPolicy(importMetadata, injector) { Interceptors =  importInterceptors});
            }
        }
    }
}