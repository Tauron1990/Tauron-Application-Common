using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Ionic.Zip;
using Tauron.Application.Files.VirtualFiles.Core;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.VirtualFiles.Zip
{
    public class InZipDirectory : DirectoryBase<InternalZipDirectory>
    {
        private InternalZipDirectory _dic;
        private ZipFile _file;

        public InZipDirectory([CanBeNull] IDirectory parentDirectory, [NotNull] string originalPath,
            [NotNull] InternalZipDirectory dic, [NotNull] ZipFile file) 
            : base(parentDirectory, originalPath)
        {
            _dic = dic;
            _file = file;
        }

        public override DateTime LastModified => _dic.ZipEntry == null ? DateTime.MinValue : _dic.ZipEntry.ModifiedTime;

        public override bool Exist => _dic.ZipEntry != null || _dic.Files.Count + _dic.Directorys.Count > 0;

        protected override void DeleteImpl()
        {
            DeleteDic(_dic, _file);
        }

        private static void DeleteDic([NotNull] InternalZipDirectory dic, [NotNull] ZipFile file)
        {
            if (dic.ZipEntry != null)
                file.RemoveEntry(dic.ZipEntry);

            foreach (var zipEntry in dic.Files)
                file.RemoveEntry(zipEntry);

            foreach (var internalZipDirectory in dic.Directorys)
                DeleteDic(internalZipDirectory, file);
        }

        protected override InternalZipDirectory GetInfo(string path)
        {
            return _dic;
        }

        public override IEnumerable<IDirectory> Directories
        {
            get
            {
                return _dic.Directorys
                    .Select(
                        internalZipDirectory =>
                            new InZipDirectory(this, OriginalPath.CombinePath(internalZipDirectory.Name),
                                internalZipDirectory, _file));
            }
        }

        public override IEnumerable<IFile> Files
        {
            get
            {
                return
                    _dic.Files.Select(
                        ent =>
                            new InZipFile(this, OriginalPath.CombinePath(InternalZipDirectory.GetFileName(ent)), _file,
                                _dic, ent));
            }
        }

        public override IFile GetFile(string name)
        {
            string[] parts = name.Split(InternalZipDirectory.PathSplit, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length <= 1)
            {
                string compledetPath = OriginalPath.CombinePath(name);
                return new InZipFile(this, compledetPath, _file, _dic, _file[compledetPath]);
            }

            InternalZipDirectory dic = _dic;
            InZipDirectory inZipParent = this;

            var originalPath = new StringBuilder(OriginalPath);

            for (int i = 0; i < parts.Length; i++)
            {
                if (i == parts.Length - 1)
                    return new InZipFile(inZipParent, originalPath.Append('\\').Append(parts[i]).ToString(), _file, dic, _file[originalPath.ToString()]);
                
                dic = dic.GetOrAdd(parts[i]);
                originalPath.Append('\\').Append(parts[i]);
                inZipParent = new InZipDirectory(inZipParent, originalPath.ToString(), dic, _file);
            }

            throw new InvalidOperationException();
        }

        protected void ResetDirectory([NotNull] ZipFile file, [NotNull] InternalZipDirectory directory)
        {
            if (file == null) throw new ArgumentNullException("file");
            if (directory == null) throw new ArgumentNullException("directory");

            _file = file;
            _dic = directory;
        }
    }
}