using Microsoft.Extensions.DependencyInjection;
using Tauron.Application.CQRS.Dispatcher.EventStore;

namespace Tauron.Application.CQRS.Dispatcher.Core.Impl
{
    public sealed class InternalObjectFactory : IObjectFactory
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public InternalObjectFactory(IServiceScopeFactory scopeFactory) 
            => _scopeFactory = scopeFactory;

        public ScopeDisposeable<DispatcherDatabaseContext> CreateDatabase()
        {
            var scope = _scopeFactory.CreateScope();
            return new ScopeDisposeable<DispatcherDatabaseContext>(scope, scope.ServiceProvider.GetRequiredService<DispatcherDatabaseContext>());
        }
    }
}