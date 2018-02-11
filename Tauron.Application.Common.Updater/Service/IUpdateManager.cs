namespace Tauron.Application.Common.Updater.Service
{
    public interface IUpdateManager
    {
        InstallerStade Setup();

        IUpdate CheckUpdate();

        void InstallUpdate(IUpdate update);

        IUpdate GetLasted();
    }
}