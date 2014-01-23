using System;
using System.Collections.Generic;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.VirtualFiles.Core
{
    public abstract class DirectoryBase<TInfo> : FileSystemNodeBase<TInfo>, IDirectory
    {
        protected DirectoryBase([CanBeNull] IDirectory parentDirectory, [NotNull] string originalPath) 
            : base(parentDirectory, true, originalPath)
        {
        }

        public abstract IEnumerable<IDirectory> Directories { get; }
        public abstract IEnumerable<IFile> Files { get; }
        public abstract IFile GetFile(string name);
    }
}