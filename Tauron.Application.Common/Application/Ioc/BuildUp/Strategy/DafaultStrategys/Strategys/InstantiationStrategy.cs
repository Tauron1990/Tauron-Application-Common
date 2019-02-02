using System.Linq;
using ExpressionBuilder;
using Tauron.Application.Ioc.Components;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
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

            context.CompilationUnit
                .AddCode(CodeLine.Assign(Operation.Variable(CompilationUnit.TargetName), policy.Constructor(context)));

            //context.Target = policy.Constructor.Invoke(context); //(context, policy.Generator);
        }

        public override void OnPerpare(IBuildContext context)
        {
            //if (context.Target != null) return;

            context.ErrorTracer.Phase = "Reciving Construtor Informations for " + context.Metadata;

            context.Policys.Add(
                new ConstructorPolicy
                {
                    Constructor =
                        context.UseInternalInstantiation()
                            ? Helper.WriteDefaultCreation(context)
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