using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Microsoft.Extensions.DependencyInjection;
using ServiceManager.Core.Services;
using Tauron.Application.CQRS.Common;

namespace ServiceManager.Core.Installation.Core
{
    public class InstallerContext : IDisposable
    {
        public const string ServiceSettingsFileName = "ServiceSettings.json";

        public const string AppSettingsFileName = "AppSettings.json";

        private RunningService? _runningService;
        private ZipArchive? _packageArchive;

        public InstallerContext(IServiceScope serviceScope, string packagePath)
        {
            ServiceScope = serviceScope;
            PackagePath = packagePath;
        }

        public Dictionary<string, object> MetaData { get; } = new Dictionary<string, object>();

        public string? PackagePath { get; }
         
        public string ServiceName { get; set; } = string.Empty;

        public IServiceScope ServiceScope { get; }

        public ZipArchive? PackageArchive
        {
            get
            {
                if (_packageArchive != null) return _packageArchive;
                try
                {
                    _packageArchive = new ZipArchive(File.Open(PackagePath, FileMode.Open), ZipArchiveMode.Read, false);
                }
                catch
                {
                    return null;
                }

                return _packageArchive;
            }
        }

        public string? ExeName { get; set; }

        public string? InstalledPath { get; set; }

        public void Dispose()
            => _packageArchive?.Dispose();

        public RunningService CreateRunningService()
            => _runningService ??= new RunningService(Guard.CheckNull(InstalledPath), ServiceStade.Ready, ServiceName, Guard.CheckNull(ExeName));

        private InstallerContext(RunningService runningService, IServiceScope serviceScope)
        {
            InstalledPath = runningService.InstallationPath;
            ExeName = runningService.Exe;
            ServiceScope = serviceScope;
            ServiceName = Guard.CheckNull(runningService.Name);
            _runningService = runningService;
        }

        public static InstallerContext CreateFrom(RunningService runningService, IServiceScope serviceScope) 
            => new InstallerContext(runningService, serviceScope);
    }
}