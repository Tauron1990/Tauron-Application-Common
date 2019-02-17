using Tauron.Application.Common.MVVM.Dynamic.Impl;
using Tauron.Application.Ioc;
using Tauron.Application.Ioc.BuildUp.Exports.DefaultExports;
using Tauron.Application.Ioc.BuildUp.Strategy;
using Tauron.Application.Ioc.Components;

namespace Tauron.Application.Common.MVVM.Dynamic
{
    public class MvvmDynamicExtension : IContainerExtension
    {
        public const string MvvmDynamicMeta = "EnableDynamicCallCreation";

        public void Initialize(ComponentRegistry components)
        {
            components.Register<IStrategy, DynamicStrategy>();
            components.Register<InternalAssemblyBuilder, InternalAssemblyBuilder>();

            var chain = components.Get<IImportSelectorChain>();

            chain.Register(new TypeSelector(components.Get<InternalAssemblyBuilder>()));
        }
    }
}