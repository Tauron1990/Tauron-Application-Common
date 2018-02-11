using System;
using System.Collections.Generic;
using System.Linq;
using Octokit;
using Tauron.Application.Common.Updater.Provider;
using Release = Tauron.Application.Common.Updater.Provider.Release;

namespace Tauron.Application.Common.Updater.Impl
{
    public sealed class GithubProvider : IUpdateProvider
    {
        private readonly string _owner;
        private readonly string _name;
        private readonly ProductHeaderValue _header;
        private readonly Func<string, Version> _versionExtractor;

        private GitHubClient _client;
        public string UpdaterFilesLocation { get; }

        public GithubProvider(string owner, string name, ProductHeaderValue header, Func<string, Version> versionExtractor, string updaterfilsLocation)
        {
            _owner = owner;
            _name = name;
            _header = header;
            _versionExtractor = versionExtractor;

            UpdaterFilesLocation = updaterfilsLocation;
            string setupFileName = "Setup.zip";
            string preperationPath = UpdaterFilesLocation.CombinePath("Step2");

            Downloader = new SimpleDownloader(UpdaterFilesLocation, setupFileName);
            Preperator = new PackagePreperator(preperationPath);
        }

        public IEnumerable<Release> GetReleases()
        {
            var client = GetClient();

            var releases = client.Repository.Release.GetAll(_owner, _name);

            foreach (var release in releases.Result)
            {
                var additialInfo = new Dictionary<string, string>
                {
                    {nameof(release.AssetsUrl), release.AssetsUrl },
                    {nameof(release.Author), release.Author.Login },
                    {nameof(release.CreatedAt), release.CreatedAt.ToString() },
                    {nameof(release.TargetCommitish), release.TargetCommitish },
                    {nameof(release.Url), release.Url },
                    {nameof(release.ZipballUrl), release.ZipballUrl }
                };

                var files = release.Assets.Select(a => new ReleaseFile(a.Name, a.BrowserDownloadUrl)).ToList();

                Version version = _versionExtractor(release.TagName);

                if(version != null)
                    yield return new Release(version, release.Body, additialInfo, files.AsReadOnly());
            }
        }

        public IDownloader Downloader { get; }
        public IPreperator Preperator { get; }

        private GitHubClient GetClient()
        {
            if (_client != null) return _client;

            _client = new GitHubClient(_header);
            return _client;
        }
    }
}