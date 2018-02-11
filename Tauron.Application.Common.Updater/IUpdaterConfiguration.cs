using System;
using Tauron.Application.Common.Updater.Provider;

namespace Tauron.Application.Common.Updater
{
    public interface IUpdaterConfiguration
    {
        IUpdateProvider Provider { get; set; }

        Version CurrentVersion { get; set; }

        string SetupFile { get; set; }

        string StartFile { get; set; }

        Action SetupCleanUp { get; set; }

        Action StartCleanUp { get; set; }

        Func<Release, ReleaseFile> FileSelector { get; set; }
    }
}