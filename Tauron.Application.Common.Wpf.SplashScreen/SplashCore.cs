using System;

namespace Tauron.Application.Common.Wpf.SplashScreen
{
    public class SplashCore : ISplashService
    {
        public SplashMessageListener Listner { get; } = SplashMessageListener.CurrentListner;

        private IUISynchronize UISync => UiSynchronize.Synchronize;

        private SplashScreen _splashScreen;

        public AppTitle AppTitle { get; set; }

        public bool ShowInTaskbar { get; set; }

        public SplashCore(AppTitle appTitle)
            : this()
            => AppTitle = appTitle;

        public SplashCore()
        {
            Listner.Width = 450;
            Listner.Height = 700;
        }

        public void CloseSplash() => UISync.Invoke(() =>
        {
            _splashScreen?.Close();
            _splashScreen = null;
        });

        public void ShowSplash()
        {
            UISync.Invoke(() =>
            {
                if(_splashScreen != null) 
                    throw new InvalidOperationException();

                _splashScreen = new SplashScreen { DataContext = this };
                _splashScreen.Show();
            });
        }
    }
}