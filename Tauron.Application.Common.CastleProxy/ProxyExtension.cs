using Tauron.Application.Common.CastleProxy.Impl;
using Tauron.Application.Ioc;
using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.Application.Ioc.BuildUp.Strategy;
using Tauron.Application.Ioc.Components;

namespace Tauron.Application.Common.CastleProxy
{
    public class ProxyExtension : IContainerExtension
    {
        public void Initialize(ComponentRegistry components)
        {
            components.Register<IMetadataFactory, MetadataFactory>(true);
            components.Register<IStrategy, InterceptionStrategy>();
        }
    }
}