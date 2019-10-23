using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using JetBrains.Annotations;
using NLog.Config;
using Tauron.Application.Implement;
using Tauron.Application.Implementation;
using Tauron.Application.Ioc;
using Tauron.Application.Views;

namespace Tauron.Application
{
    [PublicAPI]
    public static class FastStart
    {
        public const string SplashConst = "SplashScreen";

        public static string MainWindowName { get; set; } = "AppMainWindow";
        public static string ApplicationName { get; set; }

        private class FastStartApp : WpfApplication
        {
            private readonly Action<ExportResolver> _fillcontainer;
            private readonly Action<Action<SplashMessage>> _loadResources;
            private readonly Action<LoggingConfiguration> _configurateLogging;

            public FastStartApp(System.Windows.Application app, Action<ExportResolver> fillcontainer, Action<Action<SplashMessage>> loadResources, ISplashService splashService,
                Action<LoggingConfiguration> configurateLogging)
                : base(true, app, true, splashService ?? new SplashService())
            {
                _fillcontainer = fillcontainer;
                _loadResources = loadResources;
                _configurateLogging = configurateLogging;
            }

            public override IContainer Container { get; set; }
            
            public override void ConfigSplash()
            {
                if (!(Splash is SplashService)) return;

                var dic = CurrentWpfApplication.Resources;

                var control = new ContentControl
                {
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Height = 236,
                    Width = 414,
                    Content = dic[SplashConst]
                };

                SplashMessageListener.CurrentListner.SplashContent = control;
                SplashMessageListener.CurrentListner.MainLabelForeground = "Black";
                SplashMessageListener.CurrentListner.MainLabelBackground = dic["MainLabelbackground"];
            }

            protected override void ConfigurateLagging(LoggingConfiguration config, Action<SplashMessage> action)
            {
                _configurateLogging(config);
                base.ConfigurateLagging(config, action);
            }

            protected override IWindow DoStartup(CommandLineProcessor prcessor, Action<SplashMessage> action)
            {
                var temp = ViewManager.Manager.CreateWindow(MainWindowName);
                MainWindow = temp;

                CurrentWpfApplication.Dispatcher.Invoke(() =>
                {
                    Current.MainWindow = temp;
                    CurrentWpfApplication.MainWindow = (Window)temp.TranslateForTechnology();
                });
                return temp;
            }

            protected override void LoadCommands(Action<SplashMessage> action)
            {
                base.LoadCommands(action);
                CommandBinder.AutoRegister = true;
            }

            protected override void LoadResources(Action<SplashMessage> action) => _loadResources?.Invoke(action);

            public override string GetdefaultFileLocation() => GetDicPath();

            protected override void MainWindowClosed(object sender, EventArgs e) { }

            private static string GetDicPath() => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                .CombinePath($"Tauron\\{ApplicationName ?? Assembly.GetEntryAssembly().Location.GetFileNameWithoutExtension()}");

            protected override void Fill(ExportResolver container, Action<SplashMessage> action)
            {
                if(_fillcontainer != null)
                    _fillcontainer(container);
                else
                    container.AddApplicationPath();

                base.Fill(container, action);
            }
        }

        public static void Start([NotNull]System.Windows.Application app, Action<ExportResolver> fillContainer = null, CultureInfo info = null, Action<Action<SplashMessage>> loadResources = null, 
            ISplashService splashService = null, Action<LoggingConfiguration> configurateLogging = null)
        {
            WpfIuiControllerFactory.SetSynchronizationContext();
            var fastApp = new FastStartApp(Argument.NotNull(app, nameof(app)), fillContainer, loadResources, splashService, configurateLogging);
            //WpfApplicationController.Initialize(info);

            if (info != null && !info.Equals(CultureInfo.InvariantCulture))
            {
                Thread.CurrentThread.CurrentCulture = info;
                Thread.CurrentThread.CurrentUICulture = info;
            }

            fastApp.ConfigSplash();
            fastApp.InvokeOnStartup(Environment.GetCommandLineArgs());
        }

        public static void Start<TApp>(CultureInfo info = null)
            where TApp : WpfApplication, new()
        {
            var fastApp = new TApp();

            WpfApplicationController.Initialize(info);

            if (info != null && !info.Equals(CultureInfo.InvariantCulture))
            {
                Thread.CurrentThread.CurrentCulture = info;
                Thread.CurrentThread.CurrentUICulture = info;
            }

            fastApp.ConfigSplash();
            fastApp.InvokeOnStartup(Environment.GetCommandLineArgs());
        }
    }
}