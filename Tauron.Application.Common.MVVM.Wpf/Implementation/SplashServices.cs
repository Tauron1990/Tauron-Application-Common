using Tauron.Application.Implementation.Controls;

namespace Tauron.Application.Implementation
{
    public class SplashService : ISplashService
    {
        private SplashScreen _screen;
        
        public SplashService() => Listner = new SplashMessageListener();

        public SplashMessageListener Listner { get; private set; }
        
        public void CloseSplash()
        {
            var context = UiSynchronize.Synchronize;
            context.Invoke(
                () =>
                {
                    if (_screen == null) return;
                    _screen.Close();
                    _screen = null;
                });
        }
        
        public void ShowSplash()
        {
            var context = UiSynchronize.Synchronize;
            context.Invoke(
                () =>
                {
                    _screen = new SplashScreen {DataContext = Listner, Width = Listner.Width, Height = Listner.Height};
                    _screen.Show();
                });
        }
    }
}