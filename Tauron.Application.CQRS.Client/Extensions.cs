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

        public static TType? ToRealMessage<TType>(this DomainMessage message)
            where TType : class
            => JsonSerializer.Deserialize(message.EventData, Type.GetType(message.TypeName)) as TType;

        public static IMessage? ToRealMessage(this DomainMessage message)
            => JsonSerializer.Deserialize(message.EventData, Type.GetType(message.TypeName)) as IMessage;

        public static DomainMessage ToDomainMessage(this IMessage message)
        {
            var type = message.GetType();

            var msg = new DomainMessage
                      {
                          EventData = JsonSerializer.Serialize(message, message.GetType()),
                          TypeName = type.AssemblyQualifiedName,
                          EventName = type.Name,
                          EventType = EventType.Command,
                          OperationId = DateTime.UtcNow.Ticks + Random.Next()
                      };

            switch (message)
            {
                case OperationResult _:
                    msg.EventType = EventType.CommandResult;
                    break;
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


        public static void AddCqrs(this IServiceCollection serviceCollection, Action<ClientCofiguration> config)
        {
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
            serviceCollection.TryAddSingleton<ICommandSender, CommandSender>();
            serviceCollection.TryAddSingleton(typeof(GlobalEventHandler<>));
            serviceCollection.TryAddSingleton(typeof(QueryAwaiter<>));
            serviceCollection.TryAddSingleton(typeof(SimpleAwaiter<>));
            serviceCollection.TryAddSingleton<IConnectionStadeManager, ConnectionStadeManager>();

        }

        public static async Task StartCqrs(this IServiceProvider provider)
        {
            AggregateRoot.ServiceProvider = provider;

            using var scope = provider.CreateScope();
            StartDispatcher(scope.ServiceProvider);
            await scope.ServiceProvider.GetRequiredService<IHandlerManager>().Init();
        }

        private static void StartDispatcher(IServiceProvider scope)
        {
            var client = scope.GetRequiredService<IDispatcherClient>();

            Task.Run(async () => await client.Start());
        }

        public static async Task StopCqrs(this IServiceProvider provider)
        {
            AggregateRoot.ServiceProvider = null;

            using var scope = provider.CreateScope();
            await scope.ServiceProvider.GetRequiredService<IDispatcherClient>().Stop();

            AggregateRoot.AggregateLocks.Clear();
        }


        public static ClientCofiguration AddReadModel<TModel, TRespond, TQuery>(this ClientCofiguration configuration) 
            where TRespond : IQueryHelper<TQuery>
            where TQuery : IQueryResult
        {
            AddReadModel(configuration, typeof(TModel), typeof(IReadModel<TRespond, TQuery>));

            return configuration;
        }

        public static void AddFrom<TType>(this IServiceCollection serviceCollection, ClientCofiguration config)
            => ScanFrom(config, typeof(TType));

        public static ClientCofiguration AddFrom<TType>(this ClientCofiguration config)
        {
            ScanFrom(config, typeof(TType));
            return config;
        }

        public static ClientCofiguration AddType(this ClientCofiguration config, Type type)
        {
            foreach (var @interface in type.GetInterfaces())
            {
                if (!@interface.IsGenericType) continue;

                var genericDefinition = @interface.GetGenericTypeDefinition();

                if (genericDefinition == typeof(IReadModel<,>))
                {
                    AddReadModel(config, type, @interface);
                    continue;
                }

                if (genericDefinition != typeof(ICommandHandler<>) && genericDefinition != typeof(IEventHandler<>))
                    continue;

                var name = @interface.GetGenericArguments()[0].Name;

                config.RegisterHandler(name, type);
            }

            return config;
        }


        public static void AddReadModel(ClientCofiguration configuration, Type readModel, Type @interface)
            => configuration.RegisterHandler(@interface.GetGenericArguments()[0].Name, readModel);

        private static void ScanFrom(ClientCofiguration config, Type targetType)
        {
            var asm = targetType.Assembly;

            foreach (var type in asm.GetTypes())
            {
                if (!type.IsDefined(typeof(CQRSHandlerAttribute), false)) continue;

                config.AddType(type);
            }
        }

        public static void AddAwaiter<T>(this  ClientCofiguration config)
            where T : IMessage
        {
            config.RegisterHandler(typeof(T).Name, null);
        }
    }
}