using JetBrains.Annotations;

namespace Tauron.Application.Common.Updater.Service
{
    [PublicAPI]
    public interface IUpdateManager
    {
        InstallerStade Setup();

        IUpdate CheckUpdate();

        void InstallUpdate(IUpdate update);

        IUpdate GetLasted();
    }
}