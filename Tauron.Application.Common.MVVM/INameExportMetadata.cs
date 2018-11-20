using JetBrains.Annotations;

namespace Tauron.Application
{
    public interface INameExportMetadata
    {
        [NotNull]
        string Name { get; }
    }
}