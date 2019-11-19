using System;
using System.Threading.Tasks;

namespace ServiceManager.Core.Installation
{
    public class UnistallerWindowHelper
    {
        private readonly IUnistallWindow _unistallWindow;
        private readonly Func<Task<bool>> _start;

        public UnistallerWindowHelper(IUnistallWindow unistallWindow, Func<Task<bool>> start)
        {
            _unistallWindow = unistallWindow;
            _start = start;
            _unistallWindow.OnLoad += UnistallWindowOnOnLoad;
        }

        private async Task UnistallWindowOnOnLoad()
        {
            var result = await _start();

            await _unistallWindow.InvokeAsync(() => _unistallWindow.SetResult(result));
        }

        public Task<bool?> ShowDialog() => _unistallWindow.InvokeAsync(() => _unistallWindow.ShowDialog());
    }
}