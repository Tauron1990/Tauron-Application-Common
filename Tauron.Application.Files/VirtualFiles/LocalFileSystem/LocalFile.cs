using System;
using System.IO;
using Tauron.Application.Files.VirtualFiles.Core;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.VirtualFiles.LocalFileSystem
{
    public class LocalFile : FileBase<FileInfo>
    {
        public LocalFile([NotNull] string fullPath, [NotNull] IDirectory path)
            : base(path, fullPath)
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
            InfoObject.Delete();
        }

        protected override FileInfo GetInfo(string path)
        {
            return new FileInfo(path);
        }

        public override string Extension
        {
            get { return InfoObject.Extension; }
            set
            {
                if(InfoObject.Extension == value) return;

                MoveFile(InfoObject, Path.ChangeExtension(OriginalPath, value));
            }
        }

        public override string Name
        {
            get
            {
                return InfoObject.Name;
            }
            set
            {
                if(InfoObject.Name == value) return;

                MoveFile(InfoObject, ParentDirectory.OriginalPath.CombinePath(Name + Extension));
            }
        }

        public override long Size
        {
            get
            {
                return InfoObject.Length;
            }
        }

        protected override Stream CreateStream(FileAccess access, InternalFileMode mode)
        {
            return new FileStream(OriginalPath, (FileMode)mode, access, access == FileAccess.Read ? FileShare.Read : FileShare.None);
        }

        private void MoveFile([NotNull] FileInfo old, [NotNull] string newLoc)
        {
            old.MoveTo(newLoc);
        }
    }
}