using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ServiceManager.Core.Installation;
using ServiceManager.Core.Installation.Core;
using ServiceManager.Core.Services;

namespace ServiceManager.Core
{
    public sealed class InstallerWindowHelper
    {
        private readonly IInstallWindow _installWindow;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        internal bool Update { get; set; }

        internal RunningService RunningService { get; set; }

        internal string Path { get; set; }

        internal string Error { get; private set; }

        public InstallerWindowHelper(IInstallWindow installWindow, IServiceScopeFactory serviceScopeFactory)
        {
            _installWindow = installWindow;
            _serviceScopeFactory = serviceScopeFactory;

            _installWindow.OnLoad += InstallWindowOnOnLoad;
        }

        private async Task InstallWindowOnOnLoad()
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();

                var installerProcedure = scope.ServiceProvider.GetRequiredService<InstallerProcedure>();

                if (Update)
                    installerProcedure.InitUpdate(scope.ServiceProvider);
                else
                    installerProcedure.InitInstall(scope.ServiceProvider);


                await _installWindow.SyncUI(() => _installWindow.DataContext = installerProcedure);


                using var context = Update ? InstallerContext.CreateFrom(RunningService, scope) : new InstallerContext(scope, Path);

                await Task.Delay(2_000);

                var error = await installerProcedure.Install(context);
                Error = error;

                if (string.IsNullOrEmpty(error))
                {
                    await _installWindow.SyncUI(() => _installWindow.SetResult(true));
                    RunningService = context.CreateRunningService();
                }
                else
                    await _installWindow.SyncUI(() => _installWindow.SetResult(true));
            }
            catch (Exception exception)
            {
                Error = exception.Message;
                await _installWindow.SyncUI(() => _installWindow.SetResult(false));
            }
        }
    }
}