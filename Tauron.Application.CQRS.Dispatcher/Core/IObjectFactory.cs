using Tauron.Application.CQRS.Dispatcher.EventStore;

namespace Tauron.Application.CQRS.Dispatcher.Core
{
    public interface IObjectFactory
    {
        ScopeDisposeable<DispatcherDatabaseContext> CreateDatabase();
    }
}