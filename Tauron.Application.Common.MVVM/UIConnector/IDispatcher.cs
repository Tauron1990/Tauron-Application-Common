using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Tauron.Application.Models.Interfaces
{
    [PublicAPI]
    public interface IDispatcher
    {
        Thread Thread { get; }

        bool CheckAccess();

        void VerifyAccess();

        Task BeginInvoke(Delegate method, params object[] args);

        void Invoke(Action callback);

        TResult Invoke<TResult>(Func<TResult> callback);

        Task InvokeAsync(Action callback);

        Task<TResult> InvokeAsync<TResult>(Func<TResult> callback);

        object Invoke(Delegate method);

        object Invoke(Delegate method, params object[] args);
    }
}