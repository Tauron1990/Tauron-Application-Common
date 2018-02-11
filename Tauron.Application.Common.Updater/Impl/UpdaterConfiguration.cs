using System;
using Tauron.Application.Common.Updater.Provider;

namespace Tauron.Application.Common.Updater.Impl
{
    internal class UpdaterConfiguration : IUpdaterConfiguration
    {
        public IUpdateProvider Provider { get; set; }

        public Version CurrentVersion { get; set; }

        public string SetupFile { get; set; }
        public string StartFile { get; set; }

        public Action SetupCleanUp { get; set; }
        public Action StartCleanUp { get; set; }
        public Func<Release ,ReleaseFile> FileSelector { get; set; }
    }
}