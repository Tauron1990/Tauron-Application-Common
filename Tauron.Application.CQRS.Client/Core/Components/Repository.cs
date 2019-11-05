using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Tauron.Application.CQRS.Client.Domain;
using Tauron.Application.CQRS.Client.Events;
using Tauron.Application.CQRS.Client.Snapshotting;
using Tauron.Application.CQRS.Common.Configuration;
using Tauron.Application.CQRS.Common.Dto.Persistable;

namespace Tauron.Application.CQRS.Client.Core.Components
{
    public class Repository : IRepository
    {
        private readonly IInternalRepository _repository;

        public Repository(IMemoryCache memoryCache, IDispatcherApi dispatcherApi, ISnapshotStrategy snapshotStrategy, 
                          IServiceProvider serviceProvider, IOptions<ClientCofiguration> options, ISnapshotStore snapshotStore)
        {
            _repository = new CacheRepo(memoryCache,
                new SnapshotRepo(dispatcherApi, snapshotStrategy, serviceProvider, 
                                 options, snapshotStore));
        }

        public Task<T> Get<T>(Guid aggregateId) 
            where T : AggregateRoot =>
            _repository.Get<T>(aggregateId);

        public void Resfresh(AggregateRoot root) 
            => _repository.Resfresh(root);

        interface IInternalRepository
        {
            Task<T> Get<T>(Guid aggregateId)
                where T : AggregateRoot;

            void Resfresh(AggregateRoot root);
        }

        private class CacheRepo : IInternalRepository
        {
            private readonly IMemoryCache _cache;
            private readonly IInternalRepository _parent;

            public CacheRepo(IMemoryCache cache, IInternalRepository parent)
            {
                _cache = cache;
                _parent = parent;
            }

            public async Task<T> Get<T>(Guid aggregateId) 
                where T : AggregateRoot
            {
                if (_cache.Get<AggregateRoot>(aggregateId) is T root)
                    return root;

                root = await _parent.Get<T>(aggregateId);
                Resfresh(root);

                return root;
            }

            public void Resfresh(AggregateRoot root) 
                => _cache.Set(root.Id, root, TimeSpan.FromMinutes(30));
        }

        private class SnapshotRepo : IInternalRepository
        {
            private readonly IDispatcherApi _dispatcherApi;
            private readonly ISnapshotStrategy _snapshotStrategy;
            private readonly IServiceProvider _serviceProvider;
            private readonly IOptions<ClientCofiguration> _options;
            private readonly ISnapshotStore _snapshotStore;

            public SnapshotRepo(IDispatcherApi dispatcherApi, ISnapshotStrategy snapshotStrategy, IServiceProvider serviceProvider,
                                IOptions<ClientCofiguration> options, ISnapshotStore snapshotStore)
            {
                _dispatcherApi = dispatcherApi;
                _snapshotStrategy = snapshotStrategy;
                _serviceProvider = serviceProvider;
                _options = options;
                _snapshotStore = snapshotStore;
            }

            public async Task<T> Get<T>(Guid aggregateId) where T : AggregateRoot
            {
                var aggregate = AggregateFactory<T>.CreateAggregate(_serviceProvider);
                var snapshotVersion = await TryRestoreAggregateFromSnapshot(aggregateId, aggregate).ConfigureAwait(false);

                var events = (await _dispatcherApi.GetEvents(new EventsRequest(_options.Value.ApiKey, aggregateId, snapshotVersion)).ConfigureAwait(false))
                   .Select(dm => dm.ToRealMessage<IEvent>())
                   .Where(desc => desc.Version > snapshotVersion);
                
                await aggregate.LoadFromHistory(events);

                return aggregate;
            }

            private async Task<long> TryRestoreAggregateFromSnapshot<T>(Guid id, T aggregate) where T : AggregateRoot
            {
                if (!_snapshotStrategy.IsSnapshotable(typeof(T)))
                    return-1;
                await _snapshotStore.Get(id, aggregate).ConfigureAwait(false);

                return aggregate.Version;
            }

            public void Resfresh(AggregateRoot root)
            {

            }
        }
    }
}