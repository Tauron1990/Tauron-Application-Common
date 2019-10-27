using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tauron.Application.CQRS.Common.Configuration;
using Tauron.Application.CQRS.Common.Server;
using Tauron.Application.Services.Client.Commands;
using Tauron.Application.Services.Client.Events;
using Tauron.Application.Services.Client.Infrastructure;
using Tauron.Application.Services.Client.Querys;
using Tauron.Application.Services.Client.Specifications;

namespace Tauron.Application.Services.Client.Core.Components
{
    [UsedImplicitly]
    public class HandlerManager : IHandlerManager, IDisposable
    {
        private class HandlerListDelegator
        {
            private readonly List<HandlerInstace> _handlers;
            private readonly IServiceScopeFactory _serviceScopeFactory;

            public HandlerListDelegator(List<HandlerInstace> handlers, IServiceScopeFactory serviceScopeFactory)
            {
                _handlers = handlers;
                _serviceScopeFactory = serviceScopeFactory;
            }

            public async Task Handle(IMessage msg, DomainMessage rawMessage)
            {
                if (rawMessage.EventType == EventType.QueryResult)
                {
                    using var scope = _serviceScopeFactory.CreateScope();

                    var handler = (GlobalEventHandlerBase) scope.ServiceProvider.GetRequiredService(typeof(GlobalEventHandler<>).MakeGenericType(msg.GetType()));

                    await handler.Handle(msg);
                }
                else
                {
                    foreach (var handlerInstace in _handlers) await handlerInstace.Handle(msg, rawMessage);
                }
            }
        }
        private class HandlerInstace
        {
            private abstract class InvokerBase
            {
                public abstract Task<object> Invoke(IMessage msg, DomainMessage rawMessage);
            }
            private abstract class InvokerHelper : InvokerBase
            {
                private readonly Type _targetType;
                private readonly Type _targetInterface;
                private FastInvokeHandler _methodInfo;

                protected InvokerHelper(Type targetType, Type targetInterface)
                {
                    _targetType = targetType;
                    _targetInterface = targetInterface;
                }

                protected FastInvokeHandler GetMethod()
                {
                    if (_methodInfo != null) return _methodInfo;

                    var interfaceMapping = _targetType.GetInterfaceMap(_targetInterface);

                    _methodInfo = FastCall.GetMethodInvoker(interfaceMapping.InterfaceMethods.Single(m => !m.Name.StartsWith("Get")));
                    return _methodInfo;
                }
            }

            private class Command : InvokerHelper
            {
                private readonly Func<object> _handler;

                public Command(Func<object> handler, Type targetType, Type targetInterface) 
                    : base(targetType, targetInterface) => _handler = handler;

                public override async Task<object> Invoke(IMessage msg, DomainMessage rawMessage)
                {
                    return await (Task<OperationResult>) GetMethod()(_handler(), msg);
                }
            }
            private class Event : InvokerHelper
            {
                private readonly Func<object> _handler;

                public Event(Func<object> handler, Type targetType, Type targetInterface) : base(targetType, targetInterface) => _handler = handler;

                public override async Task<objectOperationResult> Invoke(IMessage msg, DomainMessage rawMessage)
                {
                    await (Task) GetMethod()(_handler(), msg);
                    return null;
                }
            }
            private class ReadModel : InvokerHelper
            {
                private readonly Func<object> _handler;

                public ReadModel(Func<object> handler, Type targetType, Type targetInterface) : base(targetType, targetInterface) => _handler = handler;

                public override async Task<object> Invoke(IMessage msg, DomainMessage rawMessage)
                {
                    var task = (Task) GetMethod()(_handler(), msg, rawMessage);
                    await task;
                    return ((dynamic) task).Result;
                }
            }

            private readonly Dictionary<Type, InvokerBase> _invoker = new Dictionary<Type, InvokerBase>();
            private readonly Func<object> _target;
            private object _realTarget;

            public bool IsCommand { get; }

            public HandlerInstace(Func<object> target, Type handlerType)
            {
                _target = target;
                var inters = handlerType.GetInterfaces();

                foreach (var i in inters.Where(i => i.IsGenericType))
                {
                    var targetType = i.GetGenericTypeDefinition();

                    if(!IsCommand)
                    {
                        IsCommand = targetType == typeof(ICommandHandler<>) ||
                                    targetType == typeof(IReadModel<,>);
                    }

                    Type key = i.GetGenericArguments()[0];

                    if (targetType == typeof(ICommandHandler<>)) _invoker[key] = new Command(() => _realTarget, handlerType, i);
                    else if (targetType == typeof(IEventHandler<>)) _invoker[key] = new Event(() => _realTarget, handlerType, i);
                    else if (targetType == typeof(IReadModel<,>)) _invoker[i.GetGenericArguments()[1]] = new ReadModel(() => _realTarget, handlerType, i);
                }

                if(_invoker.Count == 0)
                    throw new InvalidOperationException("No Invoker Found!");
            }

            public async Task Handle(IMessage msg, DomainMessage rawMessage)
            {
                _realTarget = _target();

                var provider = _realTarget as ISpecProvider<>;
                var spec = provider?.Specification(msg);
                string result = null;

                if (spec != null && !spec.IsSatisfiedBy(msg))
                    result = spec.Message;
                
                if(string.IsNullOrWhiteSpace(result))
                    await _invoker[msg.GetType()].Invoke(msg, rawMessage, token);
                else if (provider != null)
                    await provider.Error(msg, result);

                _realTarget = null;
            }
        }

        private readonly IOptions<ClientCofiguration> _configuration;
        private readonly IDispatcherClient _client;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        // ReSharper disable once CollectionNeverQueried.Local
        private readonly List<HandlerListDelegator> _handlerInstaces = new List<HandlerListDelegator>();

        public HandlerManager(IOptions<ClientCofiguration> configuration, IDispatcherClient client, IServiceScopeFactory serviceScopeFactory)
        {
            _configuration = configuration;
            _client = client;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task Init()
        {
            await _client.Connect();
            var handlerList = new List<(string, Func<IMessage, DomainMessage, Task>)>();

            foreach (var (key, value) in _configuration.Value.GetHandlers())
            {
                var commands = new List<HandlerInstace>();
                var events = new List<HandlerInstace>();

                foreach (var handlerInstace in value
                                .Select(h => new HandlerInstace(() =>
                                                                {
                                                                    using var scope = _serviceScopeFactory.CreateScope();
                                                                    return ActivatorUtilities.GetServiceOrCreateInstance(scope.ServiceProvider, h);
                                                                }, h)))
                { 
                    if(handlerInstace.IsCommand)
                        commands.Add(handlerInstace);
                    else
                        events.Add(handlerInstace);
                }


                if (commands.Count != 0)
                {
                    var del = new HandlerListDelegator(commands, _serviceScopeFactory);
                    handlerList.Add((key, del.Handle));
                    _handlerInstaces.Add(del);
                }
                else
                {
                    var del = new HandlerListDelegator(events, _serviceScopeFactory);
                    handlerList.Add((key, del.Handle));
                    _handlerInstaces.Add(del);
                }
            }

            await _client.Subscribe(handlerList);
        }

        public void Dispose() 
            => _handlerInstaces.Clear();
    }
}