using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows;
using JetBrains.Annotations;

namespace Tauron.Application.Implementation
{
    internal class WpfApplicationController : IUIController
    {
        private static ManualResetEventSlim _waiter;
        internal static bool IsInitialized;

        public void Run(IWindow window)
        {
            Application.Dispatcher.BeginInvoke(
                !(window is WpfWindow win)
                    ? new Action(() => Application.Run())
                    : () => Application.Run((Window) win.TranslateForTechnology()));
        }


        void IUIController.Shutdown() => Shutdown();

        public IWindow MainWindow
        {
            get
            {
                var win = Application.MainWindow;
                return win == null ? null : new WpfWindow(win);
            }

            set
            {
                if (value == null) Application.MainWindow = null;

                if (!(value is WpfWindow wpfwindow))
                    throw new InvalidOperationException();

                UiSynchronize.Synchronize.Invoke(() => Application.MainWindow = (Window) wpfwindow.TranslateForTechnology());
            }
        }

        public ShutdownMode ShutdownMode
        {
            get => (ShutdownMode) Application.ShutdownMode;
            set => Application.ShutdownMode = (System.Windows.ShutdownMode) value;
        }

        private static object _waiterLock = new object();

        public WpfApplicationController(System.Windows.Application app)
        {
            if(Application != null) return;
            Application = app;
        }

        internal static System.Windows.Application Application { get; private set; }

        internal static void Initialize([CanBeNull] CultureInfo info)
        {
            if(IsInitialized)
                RunApplication();

            if (Application != null) return;
            
            _waiter = new ManualResetEventSlim();
            var runner = new Thread(RunApplication) {IsBackground = false};

            if (info != null && !info.Equals(CultureInfo.InvariantCulture))
            {
                runner.CurrentCulture = info;
                runner.CurrentUICulture = info;
            }

            runner.SetApartmentState(ApartmentState.STA);
            lock (_waiterLock)
            {
                runner.Start();
                _waiter.Wait();
            }
        }

        internal static void Shutdown() => Application?.Dispatcher?.Invoke(Application.Shutdown);

        private static void RunApplication()
        {
            if(Application == null) return;
                Application = new System.Windows.Application {ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown};

            WpfIuiControllerFactory.SetSynchronizationContext();

            if(_waiter == null) return;

            _waiter.Set();
            lock (_waiterLock)
            {
                _waiter.Dispose();
                _waiter = null;
            }

            Application.Run();

            Debug.Print("WPF Application Exited");
        }
    }
}