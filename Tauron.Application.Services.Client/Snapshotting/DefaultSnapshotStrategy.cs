using System;
using Tauron.Application.Services.Client.Domain;

namespace Tauron.Application.Services.Client.Snapshotting
{
    public sealed class DefaultSnapshotStrategy : ISnapshotStrategy
    {
        public bool ShouldMakeSnapShot(AggregateRoot aggregate)
        {
            var i = aggregate.Version;

            for (var j = 0; j < aggregate.GetEvents().Count; j++)
            {
                if (++i % 100 == 0 && i != 0)
                    return true;
            }

            return false;
        }

        public bool IsSnapshotable(Type aggregateType) 
            => typeof(ISnapshotable).IsAssignableFrom(aggregateType);
    }
}