using System;
using System.Collections.Generic;
using Tauron.Application.Common.Updater.PostConfiguration;

namespace Tauron.Application.Common.Updater.Impl
{
    public sealed class PostConfigurationManager : IPostConfigurationManager
    {
        private Dictionary<Version, IConfigurator> _registry = new Dictionary<Version, IConfigurator>();

        public IPostConfigurationApplicator Applicator { get; } = new PostConfigurationApplicator();
        public IReadOnlyDictionary<Version, IConfigurator> Configurators => _registry;

        public void Register(Version version, IConfigurator configurator)
        {
            _registry = new Dictionary<Version, IConfigurator>();
        }
    }
}