
using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Tauron.Application
{
    [PublicAPI]
    public interface IUISynchronize
    {
        [NotNull]
        Task BeginInvoke([NotNull] Action action);

        [NotNull]
        Task<TResult> BeginInvoke<TResult>([NotNull] Func<TResult> action);

        void Invoke([NotNull] Action action);

        TReturn Invoke<TReturn>([NotNull] Func<TReturn> action);

        bool CheckAccess { get; }
    }

    [PublicAPI]
    public static class UiSynchronize
    {
        [NotNull]
        public static IUISynchronize Synchronize { get; set; }
    }
}