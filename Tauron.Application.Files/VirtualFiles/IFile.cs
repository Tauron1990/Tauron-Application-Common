using System.IO;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.VirtualFiles
{
    [PublicAPI]
    public interface IFile : IFileSystemNode
    {
        [NotNull]
        string Extension { get; set; }

        [NotNull]
        string Name { get; set; }

        [NotNull]
        Stream Open(FileAccess access);

        [NotNull]
        Stream Create();

        [NotNull]
        Stream CreateNew();

        long Size { get; }
    }
}