using System;
using System.Collections.Generic;

namespace Tauron.Application.Common.Updater.PostConfiguration
{
    public interface IPostConfigurationApplicator
    {
        void RunConfurationProcess(Version oldVersion, Version newVersion, IReadOnlyDictionary<Version, IConfigurator> db);
    }
}