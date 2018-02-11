using System;

namespace Tauron.Application.Common.Updater.Service
{
    public interface IInstallManager
    {
        string DownloadUpdate(IUpdate update);
        void ExecuteSetup(string installPath, string setupPath, Version oldVersion);
    }
}