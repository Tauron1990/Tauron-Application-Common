using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tauron.Application.CQRS.Client.Domain;
using Tauron.Application.CQRS.Client.Events;
using Tauron.Application.CQRS.Client.Snapshotting;

namespace Tauron.Application.CQRS.Client.Core.Components
{
    public class Session : IIternalSession
    {
        private readonly IDispatcherClient _dispatcherClient;
        private readonly ISnapshotStore _snapshotStore;
        private readonly IRepository _repository;
        private readonly ConcurrentDictionary<Guid, AggregateDescriptor> _trackedAggregates;

        public IEventPublisher EventPublisher { get; }

        public Session(IRepository repository, IDispatcherClient dispatcherClient, IEventPublisher eventPublisher, ISnapshotStore snapshotStore)
        {
            EventPublisher = eventPublisher;
            _dispatcherClient = dispatcherClient;
            _snapshotStore = snapshotStore;
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _trackedAggregates = new ConcurrentDictionary<Guid, AggregateDescriptor>();
        }

        public async Task<TType> GetAggregate<TType>(Guid id) where TType : AggregateRoot
        {
            if (_trackedAggregates.TryGetValue(id, out var descriptor))
                return (TType) descriptor.Aggregate;

            descriptor = new AggregateDescriptor();
            var aggregate = await _repository.Get<TType>(id);
            descriptor.Aggregate = aggregate;
            descriptor.Version = aggregate.Version;

            _trackedAggregates[id] = descriptor;

            return aggregate;
        }

        public async Task Commit()
        {
            try
            {
                var events = new List<IEvent>();

                foreach (AggregateDescriptor aggregateDescriptor in _trackedAggregates.Values)
                {
                    await _snapshotStore.Save(aggregateDescriptor.Aggregate);

                    events.AddRange(aggregateDescriptor.Aggregate.FlushEvents());
                    _repository.Resfresh(aggregateDescriptor.Aggregate);
                }

                await _dispatcherClient.StoreEvents(events).ConfigureAwait(false);
            }
            finally
            {
                _trackedAggregates.Clear();
            }
        }

        private class AggregateDescriptor
        {
            public AggregateRoot Aggregate { get; set; }

            public long Version { get; set; }
        }
    }
}