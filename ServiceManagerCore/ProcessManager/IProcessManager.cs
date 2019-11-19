using System.Threading.Tasks;
using ServiceManager.Core.Services;

namespace ServiceManager.Core.ProcessManager
{
    public interface IProcessManager
    {
        Task<bool> Start(RunningService service);

        Task<bool> Stop(RunningService service, int timeToKill);

        Task StartAll();

        Task StopAll();
    }
}