using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using JetBrains.Annotations;
using Tauron.Application.Models.Interfaces;

namespace Tauron.Application.UIConnector
{
    public sealed class DispatcherInterfaceImpl : IDispatcher
    {
        private readonly Dispatcher _dispatcher;

        public DispatcherInterfaceImpl([NotNull] Dispatcher dispatcher) => _dispatcher = Argument.NotNull(dispatcher, nameof(dispatcher));
        
        public Thread Thread => _dispatcher.Thread;

        public bool CheckAccess() => _dispatcher.CheckAccess();

        public void VerifyAccess() => _dispatcher.VerifyAccess();

        public Task BeginInvoke(Delegate method, params object[] args) => _dispatcher.BeginInvoke(method, args).Task;

        public void Invoke(Action callback) => _dispatcher.Invoke(callback);

        public TResult Invoke<TResult>(Func<TResult> callback) => _dispatcher.Invoke(callback);

        public Task InvokeAsync(Action callback) => _dispatcher.InvokeAsync(callback).Task;

        public Task<TResult> InvokeAsync<TResult>(Func<TResult> callback) => _dispatcher.InvokeAsync(callback).Task;

        public object Invoke(Delegate method) => _dispatcher.Invoke(method);

        public object Invoke(Delegate method, params object[] args) => _dispatcher.Invoke(method, args);
    }
}
