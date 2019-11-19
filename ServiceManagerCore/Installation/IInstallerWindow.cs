using System;
using System.Threading.Tasks;

namespace ServiceManager.Core.Installation
{
    public interface IInstallWindow
    {
        event Func<Task> OnLoad;

        object DataContext { set; }

        Task SyncUI(Action action);

        void SetResult(bool result);
    }
}