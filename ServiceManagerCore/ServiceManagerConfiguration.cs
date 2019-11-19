using System;

namespace ServiceManager.Core
{
    public sealed class ServiceManagerConfiguration
    {
        public string SettingsPath { get; set; }

        public ServiceManagerConfiguration(string settingsPath) => SettingsPath = settingsPath;

        public ServiceManagerConfiguration()
        {
            SettingsPath = String.Empty;
        }
    }
}