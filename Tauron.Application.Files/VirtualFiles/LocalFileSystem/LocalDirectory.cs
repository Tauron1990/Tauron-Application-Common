using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tauron.Application.Files.VirtualFiles.Core;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.VirtualFiles.LocalFileSystem
{
    public class LocalDirectory : DirectoryBase<DirectoryInfo>
    {

        public LocalDirectory([NotNull] string fullPath, [CanBeNull] IDirectory parentDirectory)
            : base(parentDirectory, fullPath)
        {
        }

        public override DateTime LastModified
        {
            get
            {
                return InfoObject.LastWriteTime;
            }
        }

        public override bool Exist
        {
            get
            {
                return InfoObject.Exists;
            }
        }

        protected override void DeleteImpl()
        {
            InfoObject.Delete(true);
        }

        protected override DirectoryInfo GetInfo(string path)
        {
            return new DirectoryInfo(path);
        }

        public override IEnumerable<IDirectory> Directories
        {
            get
            {
                return Directory.EnumerateDirectories(OriginalPath).Select(str => new LocalDirectory(str, this));
            }
        }

        public override IEnumerable<IFile> Files
        {
            get
            {
                return Directory.EnumerateFiles(OriginalPath).Select(str => new LocalFile(str, this));
            }
        }

        public override IFile GetFile(string name)
        {
            return new LocalFile(OriginalPath.CombinePath(name), this);
        }
    }
}