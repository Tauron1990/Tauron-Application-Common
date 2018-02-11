using Tauron.Application.Common.Updater.Provider;

namespace Tauron.Application.Common.Updater.Service
{
    public interface IUpdate
    {
        Release Release { get; }
    }
}