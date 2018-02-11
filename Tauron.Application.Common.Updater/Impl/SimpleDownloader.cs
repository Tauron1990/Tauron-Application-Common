using System;
using System.Net;
using Tauron.Application.Common.Updater.Provider;

namespace Tauron.Application.Common.Updater.Impl
{
    public sealed class SimpleDownloader : IDownloader
    {
        private readonly string _downloadLocation;
        private readonly string _fileName;
        private readonly WebClient _webClient;

        public SimpleDownloader(string downloadLocation, string fileName)
        {
            _downloadLocation = downloadLocation;
            _fileName = fileName;
            _webClient = new WebClient();
            _webClient.DownloadProgressChanged += WebClientOnDownloadProgressChanged;
        }

        private void WebClientOnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs downloadProgressChangedEventArgs)
        {
            ProgressEvent?.Invoke(this, new DownloadProgressEventArgs(downloadProgressChangedEventArgs.ProgressPercentage));
        }

        public event EventHandler<DownloadProgressEventArgs> ProgressEvent;

        public string Download(ReleaseFile release)
        {
            string path = _downloadLocation.CombinePath(_fileName);
            path.CreateDirectoryIfNotExis();

            _webClient.DownloadFileTaskAsync(release.Location, path).Wait();

            return path;
        }
    }
}