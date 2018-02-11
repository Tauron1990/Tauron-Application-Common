using System;

namespace Tauron.Application.Common.Updater.Provider
{
    public interface IDownloader
    {
        event EventHandler<DownloadProgressEventArgs> ProgressEvent;

        string Download(ReleaseFile release);
    }
}