using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Ionic.Zip;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.VirtualFiles.Zip
{
    public class InZipFileSystem : InZipDirectory, IVirtualFileSystem
    {
        private ZipFile _file;

        public InZipFileSystem([NotNull] ZipFile file)
            : base(null, "", InternalZipDirectory.ReadZipDirectory(file), file)
        {
            if (file == null) throw new ArgumentNullException("file");

            SaveAfterDispose = true;
        }

        public void Dispose()
        {
            if(SaveAfterDispose)
                _file.Save();
            _file.Dispose();
        }

        public bool IsRealTime { get { return false; } }
        public bool SaveAfterDispose { get; set; }

        public override DateTime LastModified
        {
            get
            {
                if (_file.Name.ExisFile())
                    return File.GetLastWriteTime(_file.Name);
                return base.LastModified;
            }
        }

        public string Source
        {
            get
            {
                return _file.Name;
            }
        }

        public void Reload(string source)
        {
            if(SaveAfterDispose)
                _file.Save();
            _file.Dispose();

            if (!ZipFile.IsZipFile(source)) return;

            _file = ZipFile.Read(source);
            ResetDirectory(_file, InternalZipDirectory.ReadZipDirectory(_file));
            Reset(OriginalPath, null);
        }

        public override bool Exist
        {
            get { return true; }
        }

        protected override void DeleteImpl()
        {
            if(string.IsNullOrWhiteSpace(_file.Name)) return;

            _file.Dispose();
            File.Delete(_file.Name);
        }
    }
}