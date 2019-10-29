using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tauron.Application.CQRS.Client.Domain;
using Tauron.Application.CQRS.Client.Events;
using Tauron.Application.CQRS.Client.Snapshotting;

namespace Tauron.Application.CQRS.Client.Core.Components
{
    public class Session : IInternalSession
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
            AggregateRoot.AggregateLocks.GetOrAdd(id, i => new ReaderWriterLockSlim()).EnterUpgradeableReadLock();

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
                    if (aggregateDescriptor.Aggregate.GetEvents().IsEmpty)
                        continue;

                    AggregateRoot.AggregateLocks[aggregateDescriptor.Aggregate.Id].EnterWriteLock();
                    aggregateDescriptor.WriteLocked = true;

                    await _snapshotStore.Save(aggregateDescriptor.Aggregate);

                    events.AddRange(aggregateDescriptor.Aggregate.FlushEvents());
                    _repository.Resfresh(aggregateDescriptor.Aggregate);
                }

                await _dispatcherClient.StoreEvents(events).ConfigureAwait(false);
            }
            finally
            {
                foreach (var aggregate in _trackedAggregates)
                {
                    if (aggregate.Value.WriteLocked)
                    {
                        AggregateRoot.AggregateLocks[aggregate.Key].ExitWriteLock();
                        aggregate.Value.WriteLocked = false;
                    }
                    AggregateRoot.AggregateLocks[aggregate.Key].ExitUpgradeableReadLock();
                }

                _trackedAggregates.Clear();
            }
        }

        private class AggregateDescriptor
        {
            public AggregateRoot Aggregate { get; set; }

            public long Version { get; set; }

            public bool WriteLocked { get; set; }
        }
    }
}