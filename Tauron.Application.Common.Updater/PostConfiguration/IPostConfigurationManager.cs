using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Tauron.Application.Common.Updater.PostConfiguration
{
    [PublicAPI]
    public interface IPostConfigurationManager
    {
        IPostConfigurationApplicator Applicator { get; }

        IReadOnlyDictionary<Version, IConfigurator> Configurators { get; }

        void Register(Version version, IConfigurator configurator);
    }
}