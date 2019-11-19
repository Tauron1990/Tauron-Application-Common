using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServiceManager.Core.ProcessManager;
using ServiceManager.Core.Services;

namespace ServiceManager.Core.Installation.Core
{
    public class InstallerSystem : IInstallerSystem
    {
        private readonly IProcessManager _processManager;
        private readonly ILogger<InstallerSystem> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public InstallerSystem(ILogger<InstallerSystem> logger, IServiceScopeFactory scopeFactory, IProcessManager processManager)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _processManager = processManager;
        }

        public async Task<RunningService?> Install(string path)
        {
            using var scope = _scopeFactory.CreateScope();

            var window = scope.ServiceProvider.GetRequiredService<IInstallWindow>();
            var windowHelper = new InstallerWindowHelper(window, _scopeFactory)
            {
                Path = path
            };


            if (await window.InvokeAsync(() => window.ShowDialog()) != true) return null;

            _logger.LogInformation("Install Completed");
            return windowHelper.RunningService;
        }

        public Task<bool?> Unistall(RunningService service)
        {
            using var scope = _scopeFactory.CreateScope();
            var window = scope.ServiceProvider.GetRequiredService<IUnistallWindow>();
            var helper = new UnistallerWindowHelper(window, async () =>
            {
                switch (service.ServiceStade)
                {
                    case ServiceStade.Running:
                        if (await _processManager.Stop(service, 20_000))
                            return DeleteService(service, window);
                        else
                        {
                            window.ShowError("Service Konnte nicht gestopt werden", "Fehler");
                            return false;
                        }
                    case ServiceStade.Error:
                    case ServiceStade.Ready:
                        return DeleteService(service, window);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });

            return helper.ShowDialog();
        }

        public async Task<bool?> Update(RunningService service)
        {

            using var scope = _scopeFactory.CreateScope();

            var window = scope.ServiceProvider.GetRequiredService<IInstallWindow>();
            var helper = new InstallerWindowHelper(window, _scopeFactory)
            {
                Update = true, 
                RunningService = service
            };

            var temp = await helper.ShowDialog();

            _logger.LogInformation("Update Completed");
            return temp;
        }

        private static bool DeleteService(RunningService service, IUnistallWindow window)
        {
            try
            {
                Directory.Delete(service.InstallationPath, true);
                return true;
            }
            catch (Exception e)
            {
                window.ShowError($"Fehler beim Löschen: \n {e.Message}", "Fehler");
                return false;
            }
        }
    }
}