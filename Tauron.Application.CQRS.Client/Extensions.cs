using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using RestEase;
using Tauron.Application.CQRS.Client.Commands;
using Tauron.Application.CQRS.Client.Core;
using Tauron.Application.CQRS.Client.Core.Components;
using Tauron.Application.CQRS.Client.Core.Components.Handler;
using Tauron.Application.CQRS.Client.Domain;
using Tauron.Application.CQRS.Client.Events;
using Tauron.Application.CQRS.Client.Events.Invoker;
using Tauron.Application.CQRS.Client.Infrastructure;
using Tauron.Application.CQRS.Client.Querys;
using Tauron.Application.CQRS.Client.Snapshotting;
using Tauron.Application.CQRS.Common.Configuration;
using Tauron.Application.CQRS.Common.Server;

namespace Tauron.Application.CQRS.Client
{
    public static class Extensions
    {
        private static readonly Random Random = new Random();

        public static TType ToRealMessage<TType>(this DomainMessage message)
            where TType : class
            => JsonSerializer.Deserialize(message.EventData, Type.GetType(message.TypeName)) as TType;

        public static DomainMessage ToDomainMessage(this IMessage message)
        {
            var type = message.GetType();

            var msg = new DomainMessage
                      {
                          EventData = JsonSerializer.Serialize(message),
                          TypeName = type.AssemblyQualifiedName,
                          EventName = type.Name,
                          EventType = EventType.Command,
                          SequenceNumber = DateTime.UtcNow.Ticks + Random.Next()
                      };

            switch (message)
            {
                case IQueryResult _:
                    msg.EventType = EventType.QueryResult;
                    break;
                case IQuery _:
                    msg.EventType = EventType.Query;
                    break;
                case IEvent @event:
                    msg.Id = @event.Id;
                    msg.Version = @event.Version;
                    msg.TimeStamp = @event.TimeStamp;
                    msg.EventType = EventType.Event;
                    break;
                case IAmbientEvent _:
                    msg.EventType = EventType.AmbientEvent;
                    break;
                case IAmbientCommand _:
                    msg.EventType = EventType.AmbientCommand;
                    break;
            }

            return msg;
        }


        public static void AddCqrs(this IServiceCollection serviceCollection, Action<ClientCofiguration> config = null)
        {
            //TODO AddDispatcher
            
            serviceCollection.Configure(config);
            serviceCollection.TryAddTransient(typeof(IEventInvoker<>), typeof(EventInvokerImpl<>));
            serviceCollection.AddMemoryCache();
            serviceCollection.TryAddSingleton(x => new RestClient(x.GetRequiredService<IOptions<ClientCofiguration>>().Value.PersistenceApiUrl).For<IDispatcherApi>());
            serviceCollection.TryAddTransient<ISnapshotStore, SnapshotServerStore>();
            serviceCollection.TryAddScoped<ISession, Session>();
            serviceCollection.TryAddScoped<IRepository, Repository>();
            serviceCollection.TryAddSingleton<ISnapshotStrategy, DefaultSnapshotStrategy>();
            serviceCollection.TryAddSingleton<IEventPublisher, EventPublisher>();
            serviceCollection.TryAddSingleton<IQueryProcessor, QueryProcessor>();
            serviceCollection.TryAddSingleton<IHandlerManager, HandlerManager>();
            serviceCollection.TryAddSingleton<IDispatcherClient, DispatcherClient>();
            serviceCollection.TryAddSingleton(typeof(GlobalEventHandler<>));
            serviceCollection.TryAddSingleton(typeof(QueryAwaiter<>));

        }

        public static async Task StartCqrs(this IServiceProvider provider)
        {
            AggregateRoot.ServiceProvider = provider;

            using var scope = provider.CreateScope();
            await scope.ServiceProvider.GetRequiredService<IHandlerManager>().Init();
        }

        public static async Task StopCqrs(this IServiceProvider provider)
        {
            AggregateRoot.ServiceProvider = null;

            using var scope = provider.CreateScope();
            await scope.ServiceProvider.GetRequiredService<IDispatcherClient>().Stop();

            foreach (var semaphoreSlim in AggregateRoot.AggregateLocks.Select(l => l.Value)) 
                semaphoreSlim.Dispose();

            AggregateRoot.AggregateLocks.Clear();
        }
    }
}