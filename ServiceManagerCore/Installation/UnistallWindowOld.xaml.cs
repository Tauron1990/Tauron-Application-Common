using System;
using System.Threading.Tasks;

namespace ServiceManager.Core.Installation
{
    /// <summary>
    /// Interaktionslogik für UnistallWindow.xaml
    /// </summary>
    public partial class UnistallWindow : Window
    {
        public event Func<Task<bool>> StartEvent; 

        public UnistallWindow() => InitializeComponent();

        private async void UnistallWindow_OnLoaded(object sender, RoutedEventArgs e) => DialogResult = await StartEvent.Invoke();
    }
}
