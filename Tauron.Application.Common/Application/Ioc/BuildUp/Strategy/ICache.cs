using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.Application.Ioc.LifeTime;

namespace Tauron.Application.Ioc.BuildUp.Strategy
{
    public interface ICache
    {
        void Add(ILifetimeContext context, ExportMetadata metadata, bool shareLifetime);

        [NotNull]
        ILifetimeContext GetContext(ExportMetadata metadata);
    }
}