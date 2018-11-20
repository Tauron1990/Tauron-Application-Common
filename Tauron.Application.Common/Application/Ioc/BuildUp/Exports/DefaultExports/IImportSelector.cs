using System.Collections.Generic;
using JetBrains.Annotations;

namespace Tauron.Application.Ioc.BuildUp.Exports.DefaultExports
{
    /// <summary>The ImportSelector interface.</summary>
    [PublicAPI]
    public interface IImportSelector
    {
        [NotNull]
        IEnumerable<ImportMetadata> SelectImport([NotNull] IExport exportType);
    }
}