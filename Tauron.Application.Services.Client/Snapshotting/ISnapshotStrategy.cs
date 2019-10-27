using System;
using Tauron.Application.Services.Client.Domain;

namespace Tauron.Application.Services.Client.Snapshotting
{
    public interface ISnapshotStrategy
    {

        bool ShouldMakeSnapShot(AggregateRoot aggregate);

        bool IsSnapshotable(Type aggregateType);
    }
}