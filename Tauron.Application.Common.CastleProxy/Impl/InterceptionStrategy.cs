using System;
using System.Collections.Generic;
using System.Linq;
using Castle.DynamicProxy;
using Tauron.Application.Common.CastleProxy.Impl.LifeTime;
using Tauron.Application.Ioc;
using Tauron.Application.Ioc.BuildUp.Strategy;
using Tauron.Application.Ioc.BuildUp.Strategy.DefaultStrategy;
using Tauron.Application.Ioc.LifeTime;

namespace Tauron.Application.Common.CastleProxy.Impl
{
    public class InterceptionStrategy : StrategyBase
    {
        public override void OnPostBuild(IBuildContext context)
        {
            if(context.Target == null) return;

            context.CacheEntry?.PostBuildAction.Add(OnPostBuild);

            context.ErrorTracer.Phase = "Setting up ObjectContext for " + context.Metadata;

            var contextPolicy = context.Policys.Get<ObjectContextPolicy>();

            ObjectContext objectContext;

            var name = contextPolicy.ContextName;
            if (name != null)
                objectContext = ContextManager.GetContext(name, context.Target);
            else
            {
                var holder = context.Target as IContextHolder;
                name = Guid.NewGuid().ToString();

                if (holder != null)
                {
                    if (holder.Context == null) holder.Context = ContextManager.GetContext(name, context.Target);

                    objectContext = holder.Context;
                }
                else
                {
                    var obj = context.Target;

                    objectContext = ContextManager.GetContext(name, obj);
                }
            }

            foreach (var contextProperty in contextPolicy.ContextPropertys) contextProperty.Item1.Register(objectContext, contextProperty.Item2, context.Target);

            var policy = context.Policys.Get<InterceptionPolicy>();
            if (policy == null || context.Target == null) return;
            var memberInterceptor = policy.GetParam<List<(MemberInterceptionAttribute Attribute, IInterceptor Interceptor)>>();
            if (memberInterceptor == null) return;

            foreach (var result in
                memberInterceptor.Select(mem => mem.Attribute).Where(attr => attr != null))
                result.Initialize(context.Target, objectContext, name);
        }

        public override void OnBuild(IBuildContext context)
        {
            if (!context.CanUseBuildUp()) return;

            context.CacheEntry?.BuildActions.Add(OnBuild);

            var policy = context.Policys.Get<InterceptionPolicy>();
            if (policy == null || context.Target == null) return;
            var memberInterceptor = policy.GetParam<List<(MemberInterceptionAttribute Attribute, IInterceptor Interceptor)>>();
            if (memberInterceptor == null) return;

            var options = new ProxyGenerationOptions {Selector = new InternalInterceptorSelector()};


            if (ProxyUtil.IsProxy(context.Target)) return;

            context.ErrorTracer.Phase = "Creating Proxy with Target for " + context.Metadata;


            context.Target = ProxyGeneratorFactory.ProxyGenerator
                .CreateClassProxyWithTarget(context.ExportType, context.Target, options, memberInterceptor.Select(mem => mem.Interceptor).ToArray());

            context.CacheEntry = null;
        }
        
        public override void OnPrepare(IBuildContext context)
        {
            if (!context.CanUseBuildUp()) return;

            context.CacheEntry?.PrepareActions.Add(OnPrepare);

            context.ErrorTracer.Phase = "Reciving Interception Informations for " + context.Metadata;

            var contextPolicy = new ObjectContextPolicy
            {
                ContextName = context.Metadata.Metadata?.TryGetAndCast<string>(AopConstants.ContextMetadataName)
            };

            foreach (var memberInfo in context.ExportType.GetMembers(AopConstants.DefaultBindingFlags))
            {
                var attrs =
                    memberInfo.GetAllCustomAttributes<ObjectContextPropertyAttribute>();
                foreach (var objectContextPropertyAttribute in attrs) contextPolicy.ContextPropertys.Add(Tuple.Create(objectContextPropertyAttribute, memberInfo));
            }

            context.Policys.Add(contextPolicy);

            var attr = context.Metadata.Metadata?.TryGetAndCast<InterceptAttribute>(AopConstants.InterceptMetadataName);

            object meta = false;
            if (context.Metadata.Metadata?.TryGetValue("IgnoreIntercepion", out meta) == true)
            {
                try
                {
                    if ((bool) meta)
                        return;
                }
                catch (InvalidCastException)
                {
                }
            }

            if (attr == null) return;

            List<(MemberInterceptionAttribute Attribute, IInterceptor Interceptor)> memberInterceptor = new List<(MemberInterceptionAttribute Attribute, IInterceptor Interceptor)>();

            var policy = new InterceptionPolicy();
            var temp = attr.Create();
            if (temp != null) memberInterceptor.Add((null, temp));

            memberInterceptor.AddRange(context.ExportType.GetAllCustomAttributes<MemberInterceptionAttribute>().Select(attribute => (attribute, attribute.Create(context.ExportType))));

            memberInterceptor.AddRange(from member in context.ExportType.GetMembers(AopConstants.DefaultBindingFlags)
                let intattrs = member.GetAllCustomAttributes<MemberInterceptionAttribute>()
                from interceptionAttribute in intattrs
                let temp2 = interceptionAttribute.Create(member)
                select (interceptionAttribute, temp2));

            policy.Param = memberInterceptor;
            policy.Interceptor = (build, parameters) =>
            {
                build.ErrorTracer.Phase = "Creating Direct Proxy for " + build.Metadata;
                var service = ProxyGeneratorFactory.ProxyGenerator;

                return service.CreateClassProxy(
                    build.ExportType,
                    null,
                    new ProxyGenerationOptions
                    {
                        Selector = new InternalInterceptorSelector()
                    },
                    parameters,
                    memberInterceptor.Select(mem => mem.Interceptor)
                        .ToArray());
            };

            context.Policys.Add(policy);
        }
    }
}