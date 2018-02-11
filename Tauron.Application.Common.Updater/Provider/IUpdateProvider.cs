using System.Collections.Generic;

namespace Tauron.Application.Common.Updater.Provider
{
    public interface IUpdateProvider
    {
        string UpdaterFilesLocation { get; }

        IEnumerable<Release> GetReleases();
        
        IDownloader Downloader { get; }

        IPreperator Preperator { get; }
    }
}