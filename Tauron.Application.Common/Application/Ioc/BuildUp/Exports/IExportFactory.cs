using JetBrains.Annotations;
using Tauron.Application.Ioc.Components;

namespace Tauron.Application.Ioc.BuildUp.Exports
{
    [PublicAPI]
    public interface IExportFactory : IInitializeable
    {
        string TechnologyName { get; }
    }
}