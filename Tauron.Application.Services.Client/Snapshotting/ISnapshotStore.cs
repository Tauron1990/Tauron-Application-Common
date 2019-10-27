using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Tauron.Application.Services.Client.Snapshotting
{
    public interface ISnapshotStore
    {
        Task Get(Guid id, ISnapshotable to);

        Task Save(ISnapshotable snapshot);
    }
}