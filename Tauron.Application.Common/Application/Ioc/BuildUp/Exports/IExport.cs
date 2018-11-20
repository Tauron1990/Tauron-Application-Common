using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Tauron.Application.Ioc.BuildUp.Exports
{
    [PublicAPI]
    public interface IExport : IEquatable<IExport>
    {
        IEnumerable<ExportMetadata> ExportMetadata { get; }

        IEnumerable<Type> Exports { get; }

        ExternalExportInfo ExternalInfo { get; }

        Dictionary<string, object> Globalmetadata { get; }

        Type ImplementType { get; }

        IEnumerable<ImportMetadata> ImportMetadata { get; }

        bool ShareLifetime { get; }

        ExportMetadata GetNamedExportMetadata(string contractName);

        IEnumerable<ExportMetadata> SelectContractName(string contractName);
    }
}