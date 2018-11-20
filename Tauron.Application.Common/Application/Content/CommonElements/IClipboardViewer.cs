using System;
using JetBrains.Annotations;


namespace Tauron.Application
{
    [PublicAPI]
    public interface IClipboardViewer : IDisposable
    {
        event EventHandler ClipboardChanged;
        void Initialize();
    }
}