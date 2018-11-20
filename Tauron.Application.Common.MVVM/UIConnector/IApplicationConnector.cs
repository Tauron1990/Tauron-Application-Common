using Tauron.Application.Models.Interfaces;

namespace Tauron.Application.UIConnector
{
    public interface IApplicationConnector
    {
        bool IsInDesignMode { get; }

        IDispatcher Dispatcher { get; }

        IApplication Application { get; }
    }
}