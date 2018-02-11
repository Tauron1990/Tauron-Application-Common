using System;
using System.Collections.Generic;

namespace Tauron.Application.Common.Updater.PostConfiguration
{
    public interface IPostConfigurationManager
    {
        IPostConfigurationApplicator Applicator { get; }

        IReadOnlyDictionary<Version, IConfigurator> Configurators { get; }

        void Register(Version version, IConfigurator configurator);
    }
}