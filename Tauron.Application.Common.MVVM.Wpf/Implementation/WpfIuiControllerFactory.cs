using Tauron.Application.Ioc;

namespace Tauron.Application.Implementation
{
    using App = System.Windows.Application;
    
    [Export(typeof(IUIControllerFactory))]
    public class WpfIuiControllerFactory : IUIControllerFactory
    {
        private readonly App _app;
        private IUIController _controller;

        public WpfIuiControllerFactory() {}

        public WpfIuiControllerFactory(App app, bool isInitialized)
        {
            _app = app;
            WpfApplicationController.IsInitialized = isInitialized;
        }

        void IUIControllerFactory.SetSynchronizationContext() => SetSynchronizationContext();

        public IUIController CreateController() => _controller ?? (_controller = new WpfApplicationController(_app));

        public static void SetSynchronizationContext() => UiSynchronize.Synchronize = new WPFSynchronize(App.Current.Dispatcher);
    }
}