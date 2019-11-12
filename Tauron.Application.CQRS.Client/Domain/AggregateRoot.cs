using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tauron.Application.CQRS.Client.Events;
using Tauron.Application.CQRS.Client.Events.Invoker;
using Tauron.Application.CQRS.Client.Snapshotting;
using Tauron.Application.CQRS.Common.Converter;

namespace Tauron.Application.CQRS.Client.Domain
{
    public abstract class AggregateRoot : ISnapshotable
    {
        private static ImmutableDictionary<Type, ObjectFactory> _eventFactories = ImmutableDictionary<Type, ObjectFactory>.Empty;

        internal static IServiceProvider? ServiceProvider;

        internal static readonly ConcurrentDictionary<Guid, ReaderWriterLockSlim> AggregateLocks = new ConcurrentDictionary<Guid, ReaderWriterLockSlim>();

        private ImmutableDictionary<string, ObjectInfo> _data = ImmutableDictionary<string, ObjectInfo>.Empty;
        private ImmutableList<IEvent> _events = ImmutableList<IEvent>.Empty;

        public Guid Id { get; internal set; }

        public long Version { get; internal set; }

        protected TType GetValue<TType>([CallerMemberName] string? propertyName = null)
        {
            if (string.IsNullOrWhiteSpace(propertyName)) return default!;

            if (_data.TryGetValue(propertyName, out var objectInfo) && objectInfo.Element is TType content)
                return content;

            return default!;
        }

        protected void SetValue<TType>(TType content, [CallerMemberName] string? propetyName = null)
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

            foreach (var @event in temp)
            {
                @event.Id = Id;
                @event.Version = Version++;
            }

            return temp;
        }

        internal async Task LoadFromHistory(IEnumerable<IEvent> events)
        {
            foreach (var @event in events)
            {
                var applayer = (IEventInvokerBase)GetFactory(@event.GetType())(ServiceProvider, new object[]{ this });
                await applayer.Handle(@event);

                Version = @event.Version;
            }
        }

        private static ObjectFactory GetFactory(Type eventType)
        {
            if (_eventFactories.ContainsKey(eventType))
                return _eventFactories[eventType];

            var fac = ActivatorUtilities.CreateFactory(typeof(IEventInvoker<>).MakeGenericType(eventType), new[] {typeof(IEventExecutor<>).MakeGenericType(eventType)});
            ImmutableInterlocked.TryAdd(ref _eventFactories, eventType, fac);

            return fac;
        }

        void ISnapshotable.WriteTo(Utf8JsonWriter writer) 
            => JsonSerializer.Serialize(writer, new SnapshotData(_data, Id, Version));

        void ISnapshotable.ReadFrom(ref Utf8JsonReader reader)
        {
            var data = JsonSerializer.Deserialize<SnapshotData>(ref reader);

            Id = data.Id;
            Version = data.Version;
            _data = data.ObjectInfos;
        }

        private class SnapshotData
        {
            public ImmutableDictionary<string, ObjectInfo> ObjectInfos { get; set; }

            public Guid Id { get; set; }

            public long Version { get; set; }

            public SnapshotData(ImmutableDictionary<string, ObjectInfo> objectInfos, Guid id, long version)
            {
                ObjectInfos = objectInfos;
                Id = id;
                Version = version;
            }

            public SnapshotData()
            {
                ObjectInfos = ImmutableDictionary<string, ObjectInfo>.Empty;
            }
        }
    }
}