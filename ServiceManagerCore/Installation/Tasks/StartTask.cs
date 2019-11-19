using System.Threading.Tasks;
using ServiceManager.Core.Installation.Core;
using ServiceManager.Core.ProcessManager;

namespace ServiceManager.Core.Installation.Tasks
{
    public sealed class StartTask : InstallerTask
    {
        private readonly IProcessManager _processManager;
        
        public override string Title => "Start";

        public StartTask(IProcessManager processManager) 
            => _processManager = processManager;

        public override Task Prepare(InstallerContext context)
        {
            Content = $"Starte Service: {context.ServiceName}";

            return base.Prepare(context);
        }

        public override async Task<string> RunInstall(InstallerContext context)
        {
            if(await _processManager.Start(context.CreateRunningService())) return null;

            return "Programm Konnte nicht gestartet Werden";
        }
    }
}