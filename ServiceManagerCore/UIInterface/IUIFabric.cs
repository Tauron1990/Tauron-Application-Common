using System.Threading.Tasks;
using ServiceManager.Core.Installation.Core;
using ServiceManager.Core.Installation.Tasks.Ui;

namespace ServiceManager.Core.UIInterface
{
    public interface IUIFabric
    {
        Task<object> CreateCopyTaskUI(InstallerContext context);

        Task<string?> OpenFileDialog();

        Task<object> CreateNameSelecton(InstallerContext context);
    }
}