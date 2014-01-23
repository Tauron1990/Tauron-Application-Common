using System.IO;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.VirtualFiles.Core
{
    public abstract class FileBase<TInfo> : FileSystemNodeBase<TInfo>, IFile
    {
        protected enum InternalFileMode
        {
            Open = 3,
            Create = 2,
            CreateNew = 1,
        }

        protected FileBase([NotNull] IDirectory parentDirectory,
            [NotNull] string originalPath) 
            : base(parentDirectory, true, originalPath)
        {
        }

        public abstract string Extension { get; set; }

        public abstract string Name { get; set; }

        public virtual Stream Open(FileAccess access)
        {
            return CreateStream(access, InternalFileMode.Open);
        }

        public virtual Stream Create()
        {
            return CreateStream(FileAccess.Write, InternalFileMode.Create);
        }

        public virtual Stream CreateNew()
        {
            return CreateStream(FileAccess.Write, InternalFileMode.CreateNew);
        }

        public abstract long Size { get; }

        [NotNull]
        protected abstract Stream CreateStream(FileAccess access, InternalFileMode mode);
    }
}