using System;
using System.Threading.Tasks;

namespace Tauron.Application.CQRS.Client.Snapshotting
{
    public interface ISnapshotStore
    {
        Task Get(Guid id, ISnapshotable to);

        Task Save(ISnapshotable snapshot);
    }
}