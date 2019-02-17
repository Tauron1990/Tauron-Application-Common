using Tauron.Application.Ioc.BuildUp;
using Tauron.Application.Ioc.BuildUp.Strategy;
using Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys;
using Tauron.Application.Ioc.Components;

namespace Tauron.Application.Common.MVVM.Dynamic.Impl
{
    public class DynamicStrategy : StrategyBase
    {
        private InternalAssemblyBuilder _internalAssemblyBuilder;

        public override void Initialize(ComponentRegistry components) => _internalAssemblyBuilder = components.Get<InternalAssemblyBuilder>();

        public override void OnPerpare(IBuildContext context)
        {
            if(context.Metadata.Metadata?.TryGetValue(MvvmDynamicExtension.MvvmDynamicMeta, out var meta) == false) return;

            if(!_internalAssemblyBuilder.IsBuilded)
                _internalAssemblyBuilder.Build(context.Container);

            var newType = _internalAssemblyBuilder.GetProxyType(context.Metadata.Export.ImplementType);
            if(newType == null) return;

            var policy = context.Policys.Get<ConstructorPolicy>();

            policy.Constructor = ConstructorHelper.WriteCreationFor(newType, context);
        }
    }
}