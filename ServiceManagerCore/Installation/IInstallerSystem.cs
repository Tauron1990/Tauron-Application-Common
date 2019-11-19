using System.Threading.Tasks;
using ServiceManager.Core.Services;

namespace ServiceManager.Core.Installation
{
    public interface IInstallerSystem
    {
        Task<RunningService?> Install(string path);

        Task<bool?> Unistall(RunningService service);

        Task<bool?> Update(RunningService service);
    }
}