using System;
using System.IO;
using Tauron.Application.Files.Serialization.Core;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Sources
{
    [PublicAPI]
    public sealed class FileSource : AbstractSource
    {
        private readonly string _file;

        public FileSource([NotNull] string file)
        {
            if (file == null) throw new ArgumentNullException("file");
            _file = file;
        }

        public override Stream OpenStream(FileAccess access)
        {
            return new FileStream(_file, access.HasFlag(FileAccess.Read) ? FileMode.Open : FileMode.Create, access, FileShare.None);
        }

        public override IStreamSource OpenSideLocation(string relativePath)
        {
            return new FileSource(_file.GetDirectoryName().CombinePath(relativePath));
        }
    }
}