using System;
using Tauron.Application.Common.Updater.Service;

namespace Tauron.Application.Common.Updater.Impl
{
    public class InstallManager : IInstallManager
    {
        public string DownloadUpdate(IUpdate update)
        {
            var configuration = UpdaterService.Configuration;
            var downloaded = UpdaterService.Configuration.Provider.Downloader.Download(configuration.FileSelector(update.Release));

            return downloaded;
        }

        public void ExecuteSetup(string installPath, string setupPath, Version oldVersion)
        {
            var configuration = UpdaterService.Configuration;
            configuration.Provider.Preperator.ExtractFiles(setupPath, installPath);

            UpdaterService.PostConfigurationManager.Applicator.RunConfurationProcess(oldVersion, configuration.CurrentVersion, UpdaterService.PostConfigurationManager.Configurators);

            UpdaterService.Configuration.SetupCleanUp?.Invoke();
        }
    }
}