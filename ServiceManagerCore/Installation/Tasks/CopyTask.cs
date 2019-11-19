using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ServiceManager.Core.Core;
using ServiceManager.Core.Installation.Core;
using ServiceManager.Core.UIInterface;

namespace ServiceManager.Core.Installation.Tasks
{
    public sealed class CopyTask : InstallerTask
    {
        private readonly ILogger<CopyTask> _logger;
        private readonly IUIFabric _fabric;

        public override string Title => "Daten Kopieren";

        public CopyTask(ILogger<CopyTask> logger, IUIFabric fabric)
        {
            _logger = logger;
            _fabric = fabric;
        }

        public override async Task Prepare(InstallerContext context) => Content = await _fabric.CreateCopyTaskUI(context);

        public override Task<string?> RunInstall(InstallerContext context)
        {
            var path = Path.Combine("Apps", context.ServiceName).ToApplicationPath();
            if (!Directory.Exists(path))
            {
                _logger.LogInformation($"{context.ServiceName}: Create Service Directory");
                Directory.CreateDirectory(path);
            }

            context.PackageArchive.ExtractToDirectory(path, true);

            _logger.LogInformation($"{context.ServiceName}: Extraction Compled");
            context.InstalledPath = path;

            return Task.FromResult<string>(null!)!;
        }

        public override Task Rollback(InstallerContext context)
        {
            Directory.Delete(context.InstalledPath, true);

            return Task.CompletedTask;
        }
    }
}