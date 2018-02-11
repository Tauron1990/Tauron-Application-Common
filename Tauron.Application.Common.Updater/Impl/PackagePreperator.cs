using System;
using System.IO;
using Ionic.Zip;
using SharpCompress.Compressors;
using SharpCompress.Compressors.LZMA;
using Tauron.Application.Common.Updater.Provider;

namespace Tauron.Application.Common.Updater.Impl
{
    public sealed class PackagePreperator : IPreperator
    {
        private readonly string _preperationPath;

        public PackagePreperator(string preperationPath)
        {
            _preperationPath = preperationPath;
        }

        public event EventHandler<PreperationProgressEventArgs> PreperationInProgressEvent;

        public string Prepare(string path)
        {
            ExtractFilesCommon(path, _preperationPath);

            return _preperationPath;
        }

        private void FileOnExtractProgress(object sender, ExtractProgressEventArgs extractProgressEventArgs)
        {
            switch (extractProgressEventArgs.EventType)
            {
                case ZipProgressEventType.Extracting_AfterExtractEntry:
                case ZipProgressEventType.Extracting_BeforeExtractEntry:
                    double percent = extractProgressEventArgs.EntriesTotal / 100d * extractProgressEventArgs.EntriesExtracted;

                    PreperationInProgressEvent?.Invoke(this, new PreperationProgressEventArgs(percent));
                    break;
            }
           
        }

        public void ExtractFiles(string source, string target)
        {
            ExtractFilesCommon(source, target);
        }

        private void ExtractFilesCommon(string source, string target)
        {
            using (var stream = GetSourceStream(source))
            {
                using (var file = ZipFile.Read(stream))
                {
                    file.ExtractProgress += FileOnExtractProgress;
                    file.ExtractAll(target, ExtractExistingFileAction.OverwriteSilently);
                }
            }
        }

        private Stream GetSourceStream(string source)
        {
            if(source.EndsWith("zip"))
                return new FileStream(source, FileMode.Open);

            if (source.EndsWith("lz"))
            {
                using (var stream = new LZipStream(new FileStream(source, FileMode.Open), CompressionMode.Decompress))
                {
                    MemoryStream mem = new MemoryStream();
                    stream.CopyTo(mem);
                    mem.Seek(0, SeekOrigin.Begin);

                    return mem;
                }    
            }

            throw new InvalidDataException();
        }
    }
}
