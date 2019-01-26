using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using NLog;
using NLog.Config;
using NLog.Targets;
using Tauron.Application.Implement;
using Tauron.Application.Ioc;
using Tauron.Application.Modules;

namespace Tauron.Application
{
    [PublicAPI]
    public abstract class CommonApplication
    {
        private static ITaskScheduler _scheduler;
        private static string _sourceAssembly;

        protected CommonApplication(bool doStartup, [CanBeNull] ISplashService service, [NotNull] IUIControllerFactory factory, [CanBeNull] ITaskScheduler taskScheduler = null)
        {
            Factory = Argument.NotNull(factory, nameof(factory));
            Current = this;
            _scheduler = taskScheduler ?? new TaskScheduler(UiSynchronize.Synchronize, "Application thread");
            _splash = service ?? new NullSplash();
            _doStartup = doStartup;
            SourceAssembly = new AssemblyName(Assembly.GetAssembly(GetType()).FullName).Name;
        }

        [NotNull]
        protected static string SourceAssembly
        {
            get => _sourceAssembly;
            set => _sourceAssembly = Argument.NotNull(value, nameof(value));
        }

        public static IContainer SetupTest(Action<IContainer> setup)
        {
            if (Current != null && !(Current is TestApplication))
                throw new InvalidOperationException("An Applications is Existing");

            var testApp = new TestApplication(setup);
            testApp.OnStartup(new string[0]);

            return Current.Container;
        }

        public static void FreeTest()
        {
            if (!(Current is TestApplication)) return;

            Current.Shutdown();
            // ReSharper disable once AssignNullToNotNullAttribute
            Current = null;
        }

        public virtual string GetdefaultFileLocation() => AppDomain.CurrentDomain.BaseDirectory;

        private class TestApplication : CommonApplication
        {
            private readonly Action<IContainer> _configContainer;

            public TestApplication(Action<IContainer> configContainer) : base(true, null, new UiControllerFactoryFake(), new SyncTask()) => _configContainer = configContainer;

            public override IContainer Container { get; set; }

            protected override IContainer CreateContainer()
            {
                var con = base.CreateContainer();

                _configContainer(con);

                return con;
            }

            protected override void Fill(IContainer container){}

            protected override void ConfigurateLagging(LoggingConfiguration config) => config.AddRuleForAllLevels(new VsDebuggerTarget());

            internal sealed class SyncTask : ITaskScheduler
            {
                public void Dispose() { }
                
                public Task QueueTask(ITask task)
                {
                    task.Execute();
                    return task.Task;
                }
            }

            internal sealed class UiSyncFake : IUISynchronize
            {
                public Task BeginInvoke(Action action) => QueueWorkitemAsync(action);

                public Task<TResult> BeginInvoke<TResult>(Func<TResult> action) => (Task<TResult>) QueueWorkitemAsync(action);

                public void Invoke(Action action) => action();

                public TReturn Invoke<TReturn>(Func<TReturn> action) => action();

                public bool CheckAccess { get; } = true;
            }

            internal sealed class UiControllerFactoryFake : IUIControllerFactory
            {
                public UiControllerFactoryFake() => SetSynchronizationContext();

                public IUIController CreateController() => new UiControllerFake();

                public void SetSynchronizationContext() => UiSynchronize.Synchronize = new UiSyncFake();
            }

            internal sealed class UiControllerFake : IUIController
            {
                public IWindow MainWindow { get; set; }
                public ShutdownMode ShutdownMode { get; set; }

                public void Run(IWindow window) { }

                public void Shutdown() { }
            }
        }

        public sealed class VsDebuggerTarget : TargetWithLayoutHeaderAndFooter
        {
            public VsDebuggerTarget() => Layout = "${logger}|${message}";
            
            protected override void InitializeTarget()
            {
                base.InitializeTarget();
                if (Header == null)
                    return;
                Debug.WriteLine(RenderLogEvent(Header, LogEventInfo.CreateNullEvent()));
            }

            protected override void CloseTarget()
            {
                if (Footer != null)
                    Debug.WriteLine(RenderLogEvent(Footer, LogEventInfo.CreateNullEvent()));
                base.CloseTarget();
            }
            
            protected override void Write(LogEventInfo logEvent) => Debug.WriteLine($"{RenderLogEvent(Layout, logEvent)}");
        }
        
        private class NullSplash : ISplashService
        {

            public NullSplash() => Listner = new SplashMessageListener();

            public SplashMessageListener Listner { get; private set; }
            
            public void CloseSplash() { }
            
            public void ShowSplash() { }
            
        }
        
        private readonly bool _doStartup;
        
        private readonly ISplashService _splash;
        
        private string[] _args;
        [CanBeNull]
        private IWindow _mainWindow;
        private object _mainWindowLock = new object();

        [NotNull]
        // ReSharper disable once NotNullMemberIsNotInitialized
        public static CommonApplication Current { get; private set; }
        
        [NotNull]
        public static ITaskScheduler Scheduler => _scheduler ?? (_scheduler = new TaskScheduler());
        
        //[CanBeNull]
        //public string CatalogList { get; set; }
        
        [NotNull]
        public abstract IContainer Container { get; set; }

        [NotNull]
        public IUIControllerFactory Factory { get; private set; }

        [CanBeNull]
        public IWindow MainWindow
        {
            get => _mainWindow;
            set
            {
                lock (_mainWindowLock)
                {
                    if(Equals(_mainWindow, value)) return;
                    if (_mainWindow != null)
                        _mainWindow.Closed -= MainWindowClosed;

                    _mainWindow = value;

                    if (_mainWindow != null)
                        _mainWindow.Closed += MainWindowClosed;

                    OnMainWindowChanged(value);
                }
            }
        }

        protected virtual void OnMainWindowChanged(IWindow window)
        {
        }

        [NotNull]
        public static Task QueueWorkitemAsync([NotNull] Action action, bool withDispatcher = false)
            => Scheduler.QueueTask(new UserTask(Argument.NotNull(action, nameof(action)), withDispatcher));

        public static Task QueueWorkitemAsync<TResult>([NotNull] Func<TResult> action, bool withDispatcher = false)
            => Scheduler.QueueTask(new UserResultTask<TResult>(Argument.NotNull(action, nameof(action)), withDispatcher));

        [NotNull]
        public string[] GetArgs() => (string[]) _args.Clone();

        protected virtual void ConfigurateLagging(LoggingConfiguration config) { }
        
        [NotNull]
        protected virtual IContainer CreateContainer() => new DefaultContainer();

        [CanBeNull]
        protected virtual IWindow DoStartup([NotNull] CommandLineProcessor args) => null;

        protected abstract void Fill([NotNull] IContainer container);
        
        protected virtual void LoadCommands() { }
        
        protected virtual void LoadResources() { }
        
        protected virtual void MainWindowClosed([NotNull] object sender, [NotNull] EventArgs e) { }
        
        protected virtual void OnExit() => Container.Dispose();

        protected virtual void OnStartup([NotNull] string[] args)
        {
            if (_doStartup)
            {
                _args = args;
                _splash.ShowSplash();
                _scheduler.QueueTask(new UserTask(PerformStartup, false));
            }

            if (_scheduler is TaskScheduler impl)
                impl.EnterLoop();
        }
        
        public virtual void Shutdown()
        {
            OnExit();
            Scheduler.Dispose();
        }

        protected virtual void InitializeModule([NotNull] IModule module)
        {
            Argument.NotNull(module, nameof(module));

            module.Initialize(this);
            ModuleHandlerRegistry.Progress(module);
        }
        
        private void PerformStartup()
        {
            try
            {
                var listner = _splash.Listner;

                listner.ReceiveMessage(Resources.Resources.Init_Msg_Step1);

                var config = new LoggingConfiguration();
                ConfigurateLagging(config);
                LogManager.Configuration = config;
                LogManager.ReconfigExistingLoggers();

                Container = CreateContainer();
                Fill(Container);

                listner.ReceiveMessage(Resources.Resources.Init_Msg_Step2);
                foreach (var module in Container.ResolveAll(typeof(IModule), null)
                    .Cast<IModule>()
                    .OrderBy(m => m.Order))
                    InitializeModule(module);

                listner.ReceiveMessage(Resources.Resources.Init_Msg_Step3);
                LoadResources();
                LoadCommands();

                listner.ReceiveMessage(Resources.Resources.Init_Msg_Step4);
                var win = DoStartup(new CommandLineProcessor(this));

                MainWindow = win;

                if (win != null)
                {
                    UiSynchronize.Synchronize.Invoke(
                        () =>
                        {
                            win.Show();
                            win.Closed += MainWindowClosed;
                        });
                }

                _splash.CloseSplash();
                _args = null;
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e);

                SplashMessageListener.CurrentListner.Message = e.Message;
                OnStartupError(e);
                Thread.Sleep(2000);
                Scheduler.Dispose();
                Factory.CreateController().Shutdown();
                Environment.Exit(-1);
            }
        }

        protected virtual void OnStartupError([NotNull] Exception e) { }
    }
}