using System;

namespace Tauron.Application.Common.Wpf.SplashScreen
{
    public class RichSplashCore : ISplashService
    {
        public SplashMessageListener Listner { get; } = SplashMessageListener.CurrentListner;

        private IUISynchronize UISync => UiSynchronize.Synchronize;

        private SplashScreen _splashScreen;

        public AppTitle AppTitle { get; set; }

        public bool ShowInTaskbar { get; set; }

        public RichSplashCore(AppTitle appTitle)
            : this()
            => AppTitle = appTitle;

        public RichSplashCore()
        {
            Listner.Width = 450;
            Listner.Height = 350;
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

                _splashScreen = new SplashScreen {DataContext = this, Width = Listner.Width, Height = Listner.Height};
                _splashScreen.Show();
            });
        }
    }
}