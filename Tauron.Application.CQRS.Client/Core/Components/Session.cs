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
            {
                if (descriptor.Aggregate is TType agg)
                    return agg;
                throw new InvalidOperationException("An Cached Aggregate was null");
            }

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

                foreach (var aggregateDescriptor in _trackedAggregates.Values)
                {
                    var aggregate = aggregateDescriptor.Aggregate;
                    if(aggregate == null) continue;

                    if (aggregate.GetEvents().IsEmpty)
                        continue;

                    AggregateRoot.AggregateLocks[aggregate.Id].EnterWriteLock();
                    aggregateDescriptor.WriteLocked = true;

                    await _snapshotStore.Save(aggregate);

                    events.AddRange(aggregate.FlushEvents());
                    _repository.Resfresh(aggregate);
                }

                await _dispatcherClient.StoreEvents(events).ConfigureAwait(false);
            }
            finally
            {
                foreach (var (key, aggregateDescriptor) in _trackedAggregates)
                {
                    if (aggregateDescriptor.WriteLocked)
                    {
                        AggregateRoot.AggregateLocks[key].ExitWriteLock();
                        aggregateDescriptor.WriteLocked = false;
                    }
                    AggregateRoot.AggregateLocks[key].ExitUpgradeableReadLock();
                }

                _trackedAggregates.Clear();
            }
        }

        private class AggregateDescriptor
        {
            public AggregateRoot? Aggregate { get; set; }

            public long Version { get; set; }

            public bool WriteLocked { get; set; }
        }
    }
}