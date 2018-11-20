
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;


namespace Tauron.Application.Ioc.BuildUp
{
    public interface IImportInterceptionService
    {
        [CanBeNull]
        IImportInterceptor Get([NotNull] ExportMetadata metadata, [NotNull] ImportMetadata[] imports);

    }
}