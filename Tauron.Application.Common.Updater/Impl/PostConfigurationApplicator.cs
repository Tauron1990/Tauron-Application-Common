using System;
using System.Collections.Generic;
using System.Linq;
using Tauron.Application.Common.Updater.PostConfiguration;

namespace Tauron.Application.Common.Updater.Impl
{
    public sealed class PostConfigurationApplicator : IPostConfigurationApplicator
    {
        public void RunConfurationProcess(Version oldVersion, Version newVersion, IReadOnlyDictionary<Version, IConfigurator> db)
        {
            SortedDictionary<Version, IConfigurator> sort = new SortedDictionary<Version, IConfigurator>();

            foreach (var keyValuePair in db.Where(keyValuePair => keyValuePair.Key > oldVersion && keyValuePair.Key <= newVersion))
                sort.Add(keyValuePair.Key, keyValuePair.Value);

            foreach (var pair in sort.Reverse())
                pair.Value.Apply();
        }
    }
}