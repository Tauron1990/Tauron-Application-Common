using JetBrains.Annotations;

namespace Tauron.Application.Ioc.BuildUp.Exports
{
    [PublicAPI]
    public interface IExportFactory
    {
        string TechnologyName { get; }
    }
}