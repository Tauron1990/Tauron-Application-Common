using JetBrains.Annotations;

namespace Tauron.Application
{
    [PublicAPI]
    public enum ShutdownMode
    {
        OnLastWindowClose,
        OnMainWindowClose,
        OnExplicitShutdown
    }

    [PublicAPI]
    public interface IUIController
    {
        [CanBeNull]
        IWindow MainWindow { get; set; }

        ShutdownMode ShutdownMode { get; set; }
        void Run([CanBeNull] IWindow window);

        void Shutdown();
    }
}