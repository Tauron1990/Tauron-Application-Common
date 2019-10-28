using System;
using System.Text.Json;

namespace Tauron.Application.CQRS.Client.Snapshotting
{
    public interface ISnapshotable
    {
        Guid Id { get; }

        void WriteTo(Utf8JsonWriter writer);

        void ReadFrom(ref Utf8JsonReader reader);
    }
}