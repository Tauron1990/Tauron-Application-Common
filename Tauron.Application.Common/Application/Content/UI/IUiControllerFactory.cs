using JetBrains.Annotations;


namespace Tauron.Application
{
    [PublicAPI]
    public interface IUIControllerFactory
    {
        [NotNull]
        IUIController CreateController();

        void SetSynchronizationContext();
    }
}