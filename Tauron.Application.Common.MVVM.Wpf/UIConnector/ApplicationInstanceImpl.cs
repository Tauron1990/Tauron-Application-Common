using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Tauron.Application.Models.Interfaces;
using WpfApp = System.Windows.Application;

namespace Tauron.Application.UIConnector
{
    public sealed class ApplicationInstanceImpl : IApplication
    {
        private readonly WpfApp _app;

        public ApplicationInstanceImpl(WpfApp app)
        {
            _app = Argument.NotNull(app, nameof(app));
            UiSynchronize.Synchronize.Invoke(() => _app.Exit += (sender, args) => Exit?.Invoke(sender, args));
        }

        public object FindResource(object resourceKey) => Invoke(() => _app.FindResource(resourceKey));

        public object TryFindResource(object resourceKey) => Invoke(() => _app.TryFindResource(resourceKey));

        public IEnumerable<IWindow> Windows => Invoke(() => _app.Windows.Cast<Window>().Select(w => new WpfWindow(w)));

        public IWindow MainWindow
        {
            get => CommonApplication.Current.MainWindow;
            set => CommonApplication.Current.MainWindow = value;
        }

        public event EventHandler Exit;

        private TType Invoke<TType>(Func<TType> action) => _app.Dispatcher.Invoke(action);
    }
}