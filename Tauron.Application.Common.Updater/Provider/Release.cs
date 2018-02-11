using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Tauron.Application.Common.Updater.Provider
{
    [PublicAPI]
    public sealed class Release
    {
        public Version Version { get; }
        public string Description { get; }
        public IReadOnlyDictionary<string, string> AddintionalInfo { get; }
        public IReadOnlyCollection<ReleaseFile> Files { get; }

        public Release(Version version, string description, IReadOnlyDictionary<string, string> addintionalInfo, IReadOnlyCollection<ReleaseFile> files)
        {
            Version = version;
            Description = description;
            AddintionalInfo = addintionalInfo;
            Files = files;
        }
    }
}