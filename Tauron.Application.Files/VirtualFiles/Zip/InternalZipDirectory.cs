using System;
using System.Collections.Generic;
using System.Linq;
using Ionic.Zip;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.VirtualFiles.Zip
{
    public sealed class InternalZipDirectory
    {
        public static readonly char[] PathSplit = {'/'};

        private readonly List<ZipEntry> _files;
        private readonly List<InternalZipDirectory> _directories;

        [NotNull]
        public string Name { get; private set; }

        [CanBeNull]
        public ZipEntry ZipEntry { get; private set; }

        [NotNull]
        public List<ZipEntry> Files { get { return _files; } }
 
        [NotNull]
        public List<InternalZipDirectory> Directorys { get { return _directories; } }

        private InternalZipDirectory([NotNull] string name)
        {
            Name = name;
            _files = new List<ZipEntry>();
            _directories = new List<InternalZipDirectory>();
        }

        [NotNull]
        public static InternalZipDirectory ReadZipDirectory([NotNull] ZipFile file)
        {
            if (file == null) throw new ArgumentNullException("file");
            var directory = new InternalZipDirectory(string.Empty);

            foreach (var entry in file)
            {
                Add(directory, entry);
            }

            return directory;
        }

        private static void Add([NotNull] InternalZipDirectory directory, [NotNull] ZipEntry entry)
        {
            string[] parts = entry.FileName.Split(PathSplit, StringSplitOptions.RemoveEmptyEntries);

            InternalZipDirectory mainDic = directory;

            for (int i = 0; i < parts.Length; i++)
            {
                if (i == parts.Length - 1)
                {
                    if (entry.IsDirectory)
                    {
                        mainDic = mainDic.GetOrAdd(parts[i]);
                        mainDic.ZipEntry = entry;
                    }
                    else
                        mainDic.AddFile(entry);
                }
                else
                    mainDic = mainDic.GetOrAdd(parts[i]);
            }
        }

        [NotNull]
        public static string GetFileName([NotNull] ZipEntry entry)
        {
            if (entry == null) throw new ArgumentNullException("entry");
            return entry.FileName.Split(PathSplit, StringSplitOptions.RemoveEmptyEntries).Last();
        }

        [NotNull]
        internal InternalZipDirectory GetOrAdd([NotNull] string name)
        {
            var dic = _directories.FirstOrDefault(d => d.Name == name);
            if (dic != null) return dic;
            dic = new InternalZipDirectory(name);
            _directories.Add(dic);
            return dic;
        }

        private void AddFile([NotNull] ZipEntry entry)
        {
            _files.Add(entry);
        }
    }
}