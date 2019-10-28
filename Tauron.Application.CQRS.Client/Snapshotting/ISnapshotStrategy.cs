using System;
using Tauron.Application.CQRS.Client.Domain;

namespace Tauron.Application.CQRS.Client.Snapshotting
{
    public interface ISnapshotStrategy
    {

        bool ShouldMakeSnapShot(AggregateRoot aggregate);

        bool IsSnapshotable(Type aggregateType);
    }
}