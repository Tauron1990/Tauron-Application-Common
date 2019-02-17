using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using JetBrains.Annotations;
using NLog;
using NLog.Config;
using NLog.Targets;
using Tauron.Application.Common.CastleProxy;
using Tauron.Application.Common.MVVM.Dynamic;
using Tauron.Application.Implementation;
using Tauron.Application.Ioc;

namespace Tauron.Application
{
    [PublicAPI]
    public abstract class WpfApplication : CommonApplication
    {
        public static void Run<TApp>(Action<TApp> runBeforStart = null, CultureInfo info = null) 
            where TApp : WpfApplication, new()
        {

            WpfApplicationController.Initialize(info);
            
            if (info != null && !info.Equals(CultureInfo.InvariantCulture))
            {
                Thread.CurrentThread.CurrentCulture = info;
                Thread.CurrentThread.CurrentUICulture = info;
            }

            var app = new TApp();
            runBeforStart?.Invoke(app);
            UiSynchronize.Synchronize.Invoke(() => app.ConfigSplash());
            app.OnStartup(Environment.GetCommandLineArgs());
        }

        protected WpfApplication(bool doStartup)
            : base(doStartup, new SplashService(), new WpfIuiControllerFactory()) { }


        protected WpfApplication(bool doStartup, System.Windows.Application app, bool isInit)
            : base(doStartup, new SplashService(), new WpfIuiControllerFactory(app, isInit)) { }

        protected WpfApplication(bool doStartup, ISplashService splashService)
            : base(doStartup, splashService, new WpfIuiControllerFactory()) { }


        protected WpfApplication(bool doStartup, System.Windows.Application app, bool isInit, ISplashService splashService)
            : base(doStartup, splashService, new WpfIuiControllerFactory(app, isInit)) { }

        [CanBeNull]
        public string ThemeDictionary { get; set; }

        [NotNull]
        public static System.Windows.Application CurrentWpfApplication => System.Windows.Application.Current;

        protected virtual void ConfigSplash() { }

        protected override void LoadResources(Action<SplashMessage> action)
        {
            if (string.IsNullOrEmpty(ThemeDictionary)) return;

            QueueWorkitemAsync(
                () =>
                    WpfApplicationController.Application.Resources.MergedDictionaries.Add(
                        System.Windows.Application
                            .LoadComponent(new Uri($@"/{SourceAssembly};component/{ThemeDictionary}", UriKind.Relative))
                            .SafeCast<ResourceDictionary>()),true);
        }


        protected override void MainWindowClosed(object sender, EventArgs e) => Shutdown();

        public override void Shutdown()
        {
            var app = System.Windows.Application.Current;

            app.Dispatcher.BeginInvoke(new Action(() =>
            {
                base.Shutdown();
                app.Shutdown();
            }));
        }

        protected override void ConfigurateLagging(LoggingConfiguration config, Action<SplashMessage> action)
        {
            var filetarget = new FileTarget
            {
                Name = "CommonFile",
                Layout = "${log4jxmlevent}",
                ArchiveAboveSize = 10485760,
                MaxArchiveFiles = 10,
                ArchiveFileName = GetdefaultFileLocation().CombinePath("Logs\\Tauron.Application.Common.{##}.xml"),
                FileName = GetdefaultFileLocation().CombinePath("Logs\\Tauron.Application.Common.xml"),
                ArchiveNumbering = ArchiveNumberingMode.Rolling
            };
            config.AddTarget(filetarget);

            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, filetarget));
        }

        protected override IContainer CreateContainer(Action<SplashMessage> action)
        {
            var con = base.CreateContainer(action);

            con.Register(new PropertyModelExtension());
            con.Register(new ProxyExtension());
            con.Register(new MvvmDynamicExtension());
            return con;
        }

        protected override void OnStartupError(Exception e) => MessageBox.Show(e.ToString());

        protected override void OnMainWindowChanged(IWindow window) => Factory.CreateController().MainWindow = window;
    }
}