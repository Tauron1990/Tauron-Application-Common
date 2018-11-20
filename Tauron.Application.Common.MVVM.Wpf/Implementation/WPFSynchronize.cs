using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Threading;
using JetBrains.Annotations;

namespace Tauron.Application.Implementation
{
    [PublicAPI]
    [DebuggerNonUserCode]
    public class WPFSynchronize : IUISynchronize
    {
        private readonly Dispatcher _dispatcher;
        
        public WPFSynchronize([NotNull] Dispatcher dispatcher) => _dispatcher = Argument.NotNull(dispatcher, nameof(dispatcher));

        public Task BeginInvoke(Action action) => _dispatcher.BeginInvoke(Argument.NotNull(action, nameof(action))).Task;

        public Task<TResult> BeginInvoke<TResult>(Func<TResult> action) => (Task<TResult>) _dispatcher.BeginInvoke(Argument.NotNull(action, nameof(action))).Task;

        public void Invoke(Action action) => _dispatcher.Invoke(action);

        public TReturn Invoke<TReturn>(Func<TReturn> action) => _dispatcher.Invoke(action);

        public bool CheckAccess => _dispatcher.CheckAccess();
    }
}