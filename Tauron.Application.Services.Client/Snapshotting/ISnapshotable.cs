using System;
using System.Buffers;
using System.Text.Json;
using System.Threading.Tasks;

namespace Tauron.Application.Services.Client.Snapshotting
{
    public interface ISnapshotable
    {
        Guid Id { get; }

        void WriteTo(Utf8JsonWriter writer);

        void ReadFrom(ref Utf8JsonReader reader);
    }
}