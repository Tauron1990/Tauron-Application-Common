using System.Windows;

namespace Tauron.Application.Implementation.Controls
{
    /// <summary>
    /// Interaktionslogik für SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window
    {
        public SplashScreen()
        {
            InitializeComponent();
#if(DEBUG)
            ShowInTaskbar = true;
#else
			ShowInTaskbar = false;
#endif
        }
    }
}
