using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;
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
            var dis = await AggregateRoot.AggregateLocks.GetOrAdd(id, i => new AsyncReaderWriterLock()).ReaderLockAsync();

            if (_trackedAggregates.TryGetValue(id, out var descriptor))
            {
                if (descriptor.Aggregate is TType agg)
                {
                    descriptor.Locked = dis;
                    return agg;
                }

                dis.Dispose();
                throw new InvalidOperationException("An Cached Aggregate was null");
            }

            descriptor = new AggregateDescriptor();
            var aggregate = await _repository.Get<TType>(id);
            descriptor.Aggregate = aggregate;
            descriptor.Version = aggregate.Version;
            descriptor.Locked = dis;

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

                    var locker = AggregateRoot.AggregateLocks[aggregate.Id];
                    aggregateDescriptor.Locked?.Dispose();
                    aggregateDescriptor.Locked = locker.WriterLock();

                    await _snapshotStore.Save(aggregate);

                    events.AddRange(aggregate.FlushEvents());
                    _repository.Resfresh(aggregate);
                }

                await _dispatcherClient.StoreEvents(events).ConfigureAwait(false);
            }
            finally
            {
                foreach (var (_, aggregateDescriptor) in _trackedAggregates)
                {
                    aggregateDescriptor.Locked?.Dispose();
                    aggregateDescriptor.Locked = null;
                }

                _trackedAggregates.Clear();
            }
        }

        private class AggregateDescriptor
        {
            public AggregateRoot? Aggregate { get; set; }

            public long Version { get; set; }

            public IDisposable? Locked { get; set; }
        }
    }
}