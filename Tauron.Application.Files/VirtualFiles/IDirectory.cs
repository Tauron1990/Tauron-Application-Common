using System.Collections.Generic;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.VirtualFiles
{
    public interface IDirectory : IFileSystemNode
    {
        [NotNull]
        IEnumerable<IDirectory> Directories { get; }

        [NotNull]
        IEnumerable<IFile> Files { get; }

        [NotNull]
        IFile GetFile([NotNull] string name);
    }
}