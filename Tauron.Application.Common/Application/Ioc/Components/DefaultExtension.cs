using System;
using System.ComponentModel;
using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.Application.Ioc.BuildUp.Exports.DefaultExports;
using Tauron.Application.Ioc.BuildUp.Strategy;
using Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys;

namespace Tauron.Application.Ioc.Components
{
    [Serializable]
    public sealed class DefaultExtension : IContainerExtension
    {
        public void Initialize(ComponentRegistry components)
        {
            //components.Register<IMetadataFactory, MetadataFactory>();
            components.Register<IImportSelectorChain, ImportSelectorChain>();
            components.Register<IExportFactory, DefaultExportFactory>();
            components.Register<ICache, BuildCache>();
            components.Register<IEventManager, EventManager>();
            //components.Register<IProxyService, ProxyService>();

            components.Register<IStrategy, CacheStrategy>();
            components.Register<IStrategy, LiftimeStrategy>();
            components.Register<IStrategy, InstantiationStrategy>();
            //components.Register<IStrategy, InterceptionStrategy>();
            components.Register<IStrategy, InjectionStrategy>();
            components.Register<IStrategy, NodifyBuildCompledStrategy>();

            var chain = components.Get<IImportSelectorChain>();
            chain.Register(new FieldImportSelector());
            chain.Register(new PropertyImportSelector());
            chain.Register(new MethodImportSelector());
            chain.Register(new EventImportSelector());
        }
    }
}