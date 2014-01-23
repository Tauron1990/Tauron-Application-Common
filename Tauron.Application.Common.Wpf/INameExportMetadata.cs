using System.Diagnostics.Contracts;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application
{
    public interface INameExportMetadata
    {
        [NotNull]
        string Name { get; }
    }

    internal abstract class NameExportMetadataContracts : INameExportMetadata
    {
        public string Name
        {
            get
            {
                Contract.Ensures(Contract.Result<string>() != null);
                return null;
            }
        }
    }
}