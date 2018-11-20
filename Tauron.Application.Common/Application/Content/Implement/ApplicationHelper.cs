using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using JetBrains.Annotations;
using Tauron.Application.Ioc;

namespace Tauron.Application.Implement
{
    [PublicAPI]
    [Export(typeof(IApplicationHelper))]
    public class ApplicationHelper : IApplicationHelper
    {
        [Inject]
        private IUIControllerFactory _factory;
        
        private class StartUpHelper<T>
            where T : class, IWindow
        {

            public StartUpHelper([NotNull] ApplicationHelper helper) => _helper = Argument.NotNull(helper, nameof(helper));

            [NotNull]
            public T Start()
            {
                _helper.RunUIThread(Starthelper);
                return _temp;
            }

            private void Starthelper()
            {
                _temp = Activator.CreateInstance<T>();
                _helper.RunAnonymousApplication(_temp);
            }

            private readonly ApplicationHelper _helper;

            private T _temp;

        }

        public Thread CreateUIThread(ThreadStart start)
        {
            var thread = new Thread(start) {Name = "UI Thread"};
            thread.SetApartmentState(ApartmentState.STA);
            return thread;
        }

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public IWindow RunAnonymousApplication<T>() where T : class, IWindow => new StartUpHelper<T>(this).Start();

        public void RunAnonymousApplication(IWindow window)
        {
            var app = _factory.CreateController();
            app.MainWindow = window;
            app.ShutdownMode = ShutdownMode.OnMainWindowClose;
            app.Run(window);
        }

        public void RunUIThread(ThreadStart start) => CreateUIThread(start).Start();
    }
}