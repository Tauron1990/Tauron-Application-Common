using System;
using System.Linq;
using Tauron.Application.Ioc.Components;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DefaultStrategy
{
    public class InstantiationStrategy : StrategyBase
    {
        private IImportInterceptionService[] _services;

        public override void Initialize(ComponentRegistry components) => _services = components.GetAll<IImportInterceptionService>().ToArray();

        public override void OnCreateInstance(IBuildContext context)
        {
            var policy = context.Policys.Get<ConstructorPolicy>();
            if (policy == null) return;

            context.ErrorTracer.Phase = "Contruct Object for " + context.Metadata;

            var constructor = policy.Constructor;
            context.Target = constructor.Invoke(context); //(context, policy.Generator);

            context.CacheEntry?.InstanceActions.Add(c => CreateInst(c, constructor));
        }

        private static void CreateInst(IBuildContext context, Func<IBuildContext, object> constructor) 
            => context.Target = constructor(context);

        public override void OnPrepare(IBuildContext context)
        {
            if (context.Target != null) return;

            context.ErrorTracer.Phase = "Reciving Construtor Informations for " + context.Metadata;

            context.Policys.Add(
                new ConstructorPolicy
                {
                    Constructor =
                        context.UseInternalInstantiation()
                            ? ConstructorHelper.WriteDefaultCreation(context)
                            : context.Metadata.Export.ExternalInfo.Create
                });

            var pol = context.Policys.Get<ExternalImportInterceptorPolicy>();


            foreach (var service in _services.Select(i => i.Get(context.Metadata, context.Metadata.Export.ImportMetadata.ToArray())).Where(i => i != null))
            {
                if (pol == null)
                {
                    pol = new ExternalImportInterceptorPolicy();
                    context.Policys.Add(pol);
                }

                pol.Interceptors.Add(service);
            }
        }
    }
}