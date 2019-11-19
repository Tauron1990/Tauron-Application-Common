using System;
using System.Threading.Tasks;

namespace ServiceManager.Core.Installation
{
    public interface IUnistallWindow
    {
        event Func<Task> OnLoad;

        Task InvokeAsync(Action action);

        Task<T> InvokeAsync<T>(Func<T> action);

        void ShowError(string message, string title);

        void SetResult(bool result);

        bool? ShowDialog();
    }
}