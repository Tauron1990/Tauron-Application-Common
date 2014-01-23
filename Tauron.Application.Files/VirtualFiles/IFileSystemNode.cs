using System;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.VirtualFiles
{
    [PublicAPI]
    public interface IFileSystemNode
    {
        [NotNull]
        string OriginalPath { get; }

        DateTime LastModified { get; }

        [CanBeNull]
        IDirectory ParentDirectory { get; }

        void Delete();

        bool IsDirectory { get; }

        bool Exist { get; }
    }
}