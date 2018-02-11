using System;
using JetBrains.Annotations;
using Octokit;
using Tauron.Application.Common.Updater.Impl;
using Tauron.Application.Common.Updater.PostConfiguration;
using Tauron.Application.Common.Updater.Service;

namespace Tauron.Application.Common.Updater
{
    [PublicAPI]
    public static class UpdaterService
    {
        public static IUpdaterConfiguration Configuration { get; } = new UpdaterConfiguration();

        public static IUpdateManager UpdateManager { get; } = new UpdateManager();

        public static IInstallManager InstallManager { get; } = new InstallManager();

        public static IPostConfigurationManager PostConfigurationManager { get; } = new PostConfigurationManager();

        public static void SetGithubProvider(string owner, string name, Tuple<string, string> header, Func<string, Version> versionExtractor, string updaterfilesLocation = null)
        {
            Configuration.Provider = new GithubProvider(owner, name, new ProductHeaderValue(header.Item1, header.Item2), versionExtractor, updaterfilesLocation ?? AppDomain.CurrentDomain.BaseDirectory.CombinePath("Updater"));
        }
    }
}