using System;
using System.IO;
using Ionic.Zip;
using Tauron.Application.Files.VirtualFiles.Core;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.VirtualFiles.Zip
{
    public sealed class InZipFile : FileBase<ZipEntry>
    {
        private class ZipWriteHelper : MemoryStream
        {
            private readonly string _entry;
            private readonly ZipFile _file;
            private readonly Action<ZipEntry> _updateAction;

            public ZipWriteHelper([NotNull] string entry, [NotNull] ZipFile file, [NotNull] byte[] buffer,
                [NotNull]Action<ZipEntry> updateAction)
                : base(buffer)
            {
                _entry = entry;
                _file = file;
                _updateAction = updateAction;
            }

            protected override void Dispose(bool disposing)
            {
                _updateAction(_file.UpdateEntry(_entry, GetBuffer()));

                base.Dispose(disposing);
            }
        }

        private readonly ZipFile _file;
        private readonly InternalZipDirectory _directory;
        private readonly ZipEntry _entry;

        public InZipFile([NotNull] IDirectory parentDirectory, [NotNull] string originalPath, 
            [NotNull] ZipFile file, [NotNull] InternalZipDirectory directory, [CanBeNull] ZipEntry entry)
            : base(parentDirectory, originalPath)
        {
            _file = file;
            _directory = directory;
            _entry = entry;
        }


        public override DateTime LastModified
        {
            get
            {
                return InfoObject.LastModified;
            }
        }

        public override bool Exist
        {
            get
            {
                return InfoObject == null;
            }
        }

        protected override void DeleteImpl()
        {
            _file.RemoveEntry(InfoObject);
            Reset(OriginalPath, ParentDirectory);
        }

        protected override ZipEntry GetInfo(string path)
        {
            return _entry;
        }

        public override string Extension
        {
            get
            {
                return InfoObject.FileName.GetExtension();
            }
            set
            {
                Name = Path.ChangeExtension(Name, value);
            }
        }

        public override string Name
        {
            get { return InfoObject.FileName; } 
            set { InfoObject.FileName = value; }
        }

        public override long Size
        {
            get
            {
                return InfoObject.UncompressedSize;
            }
        }

        protected override Stream CreateStream(FileAccess access, InternalFileMode mode)
        {
            switch (access)
            {
                case FileAccess.Read:
                    return InfoObject.OpenReader();
                case FileAccess.Write:
                case FileAccess.ReadWrite:
                    using (var stream = new MemoryStream())
                    {
                        if (!Exist) return new ZipWriteHelper(OriginalPath, _file, new byte[0], ZipEntryUpdated);

                        InfoObject.Extract(stream);
                        return new ZipWriteHelper(InfoObject.FileName, _file, stream.GetBuffer(), ZipEntryUpdated);
                    }
                default:
                    throw new ArgumentOutOfRangeException("access");
            }
        }

        private void ZipEntryUpdated([NotNull] ZipEntry zipEntry)
        {
            _directory.Files.RemoveAt(_directory.Files.FindIndex(ent => ent.FileName == zipEntry.FileName));
            Reset(OriginalPath, ParentDirectory);
        }
    }
}