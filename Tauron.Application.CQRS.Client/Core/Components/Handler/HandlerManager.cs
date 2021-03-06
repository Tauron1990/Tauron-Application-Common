﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Tauron.Application.CQRS.Client.Commands;
using Tauron.Application.CQRS.Client.Events;
using Tauron.Application.CQRS.Client.Querys;
using Tauron.Application.CQRS.Common.Configuration;

namespace Tauron.Application.CQRS.Client.Core.Components.Handler
{
    [UsedImplicitly]
    public class HandlerManager : IHandlerManager, IDisposable
    {
        private class HandlerFactory
        {
            private readonly IServiceProvider _serviceProvider;
            private readonly Type _interfaceType;
            private readonly Type _targetType;
            private readonly Type _messageType;

            public HandlerFactory(IServiceProvider serviceProvider, Type interfaceType, Type targetType)
            {
                _serviceProvider = serviceProvider;
                _interfaceType = interfaceType;
                _targetType = targetType;
                _messageType = interfaceType.GetGenericArguments()[0];
            }

            public HandlerBase Create()
            {
                return (HandlerBase) ActivatorUtilities.CreateInstance(_serviceProvider, typeof(MessageHandler<>).MakeGenericType(_messageType),
                    InstanceCreator(_targetType), _targetType, _interfaceType);
            }

            private Func<object> InstanceCreator(Type targetType)
            {
                return () =>
                {
                    using var scope = _serviceProvider.CreateScope();
                    return ActivatorUtilities.GetServiceOrCreateInstance(scope.ServiceProvider, targetType);
                };
            }
        }

        private readonly IOptions<ClientCofiguration> _configuration;
        private readonly IDispatcherClient _client;
        private readonly IServiceProvider _serviceProvider;
        // ReSharper disable once CollectionNeverQueried.Local
        private readonly List<HandlerListDelegator> _handlerInstaces = new List<HandlerListDelegator>();

        public HandlerManager(IOptions<ClientCofiguration> configuration, IDispatcherClient client, IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _client = client;
            _serviceProvider = serviceProvider;
        }

        public async Task Init()
        {
            await _client.Connect();
            var handlerList = new List<(string, MessageHandler)>();

            foreach (var (key, value) in _configuration.Value.GetHandlers())
            {
                var handlers = (from targetType in value 
                                from @interface in targetType.GetInterfaces().Where(i => i.IsGenericType) 
                                let typeDef = @interface.GetGenericTypeDefinition() 
                                where typeDef == typeof(ICommandHandler<>) || typeDef == typeof(IEventHandler<>) || typeDef == typeof(IReadModel<,>) 
                                select new Func<HandlerBase>(new HandlerFactory(_serviceProvider, @interface, targetType).Create))
                   .ToList();


                var del = new HandlerListDelegator(handlers, _serviceProvider.GetRequiredService<IServiceScopeFactory>());
                handlerList.Add((key, del.Handle));
                _handlerInstaces.Add(del);
            }

            await _client.Subscribe(handlerList);
        }

        public void Dispose()
            => _handlerInstaces.Clear();
    }
}