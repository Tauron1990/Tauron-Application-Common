using System;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.VirtualFiles.Core
{
    public abstract class FileSystemNodeBase<TInfo> : IFileSystemNode
    {
        public abstract DateTime LastModified { get; }

        public IDirectory ParentDirectory { get; private set; }

        private Lazy<TInfo> _infoObject; 

        protected TInfo InfoObject => _infoObject.Value;

        public void Delete()
        {
            if(Exist)
                DeleteImpl();
        }

        public bool IsDirectory { get; private set; }

        public string OriginalPath { get; private set; }

        public abstract bool Exist { get; }

        protected abstract void DeleteImpl();

        [CanBeNull]
        protected abstract TInfo GetInfo([NotNull] string path);

        protected FileSystemNodeBase([CanBeNull] IDirectory parentDirectory, bool isDirectory, 
            [NotNull] string originalPath)
        {
            ParentDirectory = parentDirectory;
            OriginalPath = originalPath;
            _infoObject = new Lazy<TInfo>(() => GetInfo(OriginalPath));
        }

        protected virtual void Reset(string path, IDirectory parent)
        {
            ParentDirectory = parent;
            OriginalPath = path;
            _infoObject = new Lazy<TInfo>(() => GetInfo(OriginalPath));
        }
    }
}