using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ServiceManager.Core.Installation.Core;
using ServiceManager.Core.UIInterface;

namespace ServiceManager.Core.Installation.Tasks
{
    public sealed class SelectUpdateTask : InstallerTask
    {
        private readonly ILogger<SelectUpdateTask> _logger;
        private readonly IUIFabric _fabric;
        private ZipArchive? _zipArchive;

        public override string Title => "Datei Wählen";

        public SelectUpdateTask(ILogger<SelectUpdateTask> logger, IUIFabric fabric)
        {
            _logger = logger;
            _fabric = fabric;
        }

        public override Task Prepare(InstallerContext context)
        {
            Content = "Update Packet Wählen";

            return base.Prepare(context);
        }

        public override async Task<string?> RunInstall(InstallerContext context)
        {
            var service = context.CreateRunningService();

             var path = await _fabric.OpenFileDialog();

            if (path == null)
            {
                _logger.LogWarning($"{service.Name}: Update File not Selected");
                return "Es wurde Keine Datei fürs Update gewählt.";
            }

            try
            {
                var archive = new ZipArchive(new FileStream(path, FileMode.Open));

                context.MetaData[MetaKeys.UpdateFile] = path;
                context.MetaData[MetaKeys.ArchiveFile] = archive;

                _zipArchive = archive;
            }
            catch
            {
                _logger.LogWarning($"{service.Name}: Error Open Update File");
                return "Die Datei konnte nicht geöffnet werden";
            }

            return null;
        }

        public override void Dispose() 
            => _zipArchive?.Dispose();
    }
}