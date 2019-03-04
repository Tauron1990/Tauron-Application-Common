using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.Application.Ioc.BuildUp.Strategy.DefaultStrategy;
using Tauron.Application.Ioc.LifeTime;

namespace Tauron.Application.Ioc.BuildUp.Strategy
{
    public interface ICache
    {
        void Add(ILifetimeContext context, ExportMetadata metadata, bool shareLifetime);
        ILifetimeContext GetContext(ExportMetadata metadata);

        FactoryCacheEntry Get(ExportMetadata metadata);
        void Set(ExportMetadata metadata, FactoryCacheEntry entry);
    }
}