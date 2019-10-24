using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Tauron.Application.Services.Client.Snapshotting;

namespace Tauron.Application.Services.Client.Domain
{
    public abstract class AggregateRoot : ISnapshotable
    {
        private readonly Dictionary<string, object> _data = new Dictionary<string, object>();

        public Guid Id { get; internal set; }

        void ISnapshotable.WriteTo(Utf8JsonWriter writer)
        {
            
        }

        void ISnapshotable.ReadFrom(Utf8JsonReader reader)
        {
            
        }
    }
}