using System;
using System.IO;
using Tauron.Application.Files.Serialization.Core;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Sources
{
    [PublicAPI]
    public abstract class AbstractSource : IStreamSource
    {
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            
        }

        ~AbstractSource()
        {
            Dispose(false);
        }

        public abstract Stream OpenStream(FileAccess access);

        public abstract IStreamSource OpenSideLocation(string relativePath);
    }
}