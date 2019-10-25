using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using Tauron.Application.CQRS.Common.Converter;
using Tauron.Application.Services.Client.Events;
using Tauron.Application.Services.Client.Snapshotting;

namespace Tauron.Application.Services.Client.Domain
{
    public abstract class AggregateRoot : ISnapshotable
    {
        internal static 

        private ImmutableDictionary<string, ObjectInfo> _data = ImmutableDictionary<string, ObjectInfo>.Empty;
        private ImmutableList<IEvent> _events = ImmutableList<IEvent>.Empty;

        public Guid Id { get; internal set; }

        protected TType GetValue<TType>([CallerMemberName] string propertyName = null)
        {
            if (string.IsNullOrWhiteSpace(propertyName)) return default;

            if (_data.TryGetValue(propertyName, out var objectInfo) && objectInfo.Element is TType content)
                return content;

            return default;
        }

        protected void SetValue<TType>(TType content, [CallerMemberName] string propetyName = null)
        {
            if(string.IsNullOrWhiteSpace(propetyName)) return;

            if (_data.TryGetValue(propetyName, out var objectInfo))
                objectInfo.Element = content;
            else
                _data = _data.Add(propetyName, new ObjectInfo {Element = content});
        }

        protected async Task ProvideEvent<TEvent>(TEvent @event)
        where TEvent : class, IEvent
        {
            _events = _events.Add(@event);
            if (this is IEventExecutor<TEvent> handler)
                await handler.Apply(@event);
        }

        public ImmutableList<IEvent> GetEvents() => _events;

        internal ImmutableList<IEvent> FlushEvents()
        {
            var temp = _events;
            _events = ImmutableList<IEvent>.Empty;
            return temp;
        }

        internal async Task LoadFromHistory(IEnumerable<IEvent> events)
        {
            foreach (var @event in events)
            {
                if (this is IEventExecutor<TEvent> handler)
                    await handler.Apply(@event);
            }
        }

        void ISnapshotable.WriteTo(Utf8JsonWriter writer) 
            => JsonSerializer.Serialize(writer, _data);

        void ISnapshotable.ReadFrom(ref Utf8JsonReader reader) 
            => _data = JsonSerializer.Deserialize< ImmutableDictionary <string, ObjectInfo>>(ref reader);
    }
}