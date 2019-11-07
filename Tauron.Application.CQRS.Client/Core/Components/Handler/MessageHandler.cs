using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tauron.Application.CQRS.Client.Commands;
using Tauron.Application.CQRS.Client.Domain;
using Tauron.Application.CQRS.Client.Events;
using Tauron.Application.CQRS.Client.Infrastructure;
using Tauron.Application.CQRS.Client.Querys;
using Tauron.Application.CQRS.Client.Specifications;
using Tauron.Application.CQRS.Client.Specifications.Fluent;
using Tauron.Application.CQRS.Common.Server;

namespace Tauron.Application.CQRS.Client.Core.Components.Handler
{
    public sealed class MessageHandler<TMessage> : TypedHandlerBase<TMessage> where TMessage : IMessage
    {
        private abstract class InvokerBase
        {
            public abstract Task<object> Invoke(IMessage msg);
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

            public override async Task<object> Invoke(IMessage msg) 
                => await (Task<OperationResult>)GetMethod()(_handler(), msg);
        }
        private class Event : InvokerHelper
        {
            private readonly Func<object> _handler;

            public Event(Func<object> handler, Type targetType, Type targetInterface) : base(targetType, targetInterface) => _handler = handler;

            public override async Task<object> Invoke(IMessage msg)
            {
                await (Task)GetMethod()(_handler(), msg);
                return null;
            }
        }
        private class ReadModel : InvokerHelper
        {
            private readonly Func<object> _handler;

            public Type QueryResult { get; }

            public ReadModel(Func<object> handler, Type targetType, Type targetInterface)
                : base(targetType, targetInterface)
            {
                _handler = handler;
                QueryResult = targetInterface.GetGenericArguments()[1];
            }

            public override async Task<object> Invoke(IMessage msg)
            {
                var task = (Task)GetMethod()(_handler(), msg);
                await task;
                return ((dynamic)task).Result;
            }
        }

        private readonly InvokerBase _invoker;
        private readonly Func<object> _target;
        private readonly ISession _session;
        private readonly ILogger<HandlerBase> _logger;
        private readonly IDispatcherClient _dispatcherClient;
        private object _realTarget;

        public MessageHandler(ILogger<HandlerBase> logger, IDispatcherClient dispatcherClient, ISession session, Func<object> target, Type handlerType, Type inter)
        {
            _target = target;
            _session = session;
            _logger = logger;
            _dispatcherClient = dispatcherClient;

                var targetType = inter.GetGenericTypeDefinition();

                if (targetType == typeof(ICommandHandler<>)) _invoker = new Command(() => _realTarget, handlerType, inter);
                else if (targetType == typeof(IEventHandler<>)) _invoker = new Event(() => _realTarget, handlerType, inter);
                else if (targetType == typeof(IReadModel<,>)) _invoker = new ReadModel(() => _realTarget, handlerType, inter);
        }

        protected override async Task HandleInternal(TMessage msg, DomainMessage rawMessage)
        {
            Type queryResult = null;

            _realTarget = _target();
            var commandHandler = _realTarget as CommandHandlerBase; 
            if(commandHandler != null)
                commandHandler.Session = _session;

            try
            {
                var invoker = _invoker;
                switch (invoker)
                {
                    case ReadModel readModel:
                        queryResult = readModel.QueryResult;
                        await _dispatcherClient.SendToClient(rawMessage.Sender, (IMessage) (await invoker.Invoke(msg)));
                        break;
                    case Command command:
                        if (_realTarget is ISpecProvider<TMessage> specProvider)
                        {
                            var specification = specProvider.Get(new GenericSpecification<TMessage>());
                            if (specification != null)
                            {
                                var result = await specification.IsSatisfiedBy(msg);
                                if (result.Error)
                                {
                                    await _dispatcherClient.SendToClient(rawMessage.Sender, result, rawMessage.OperationId);
                                    return;
                                }
                            }
                        }

                        await _dispatcherClient.SendToClient(rawMessage.Sender, (OperationResult) (await command.Invoke(msg)), rawMessage.OperationId);
                        break;
                    case Event @event:
                        await @event.Invoke(msg);
                        break;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while Handle Message");
                switch (msg)
                {
                    case ICommand _:
                        await _dispatcherClient.SendToClient(rawMessage.Sender, OperationResult.Failed(OperationError.Error(-1, $"{e.GetType()} -- {e.Message}")), rawMessage.OperationId^);
                        break;
                    case IQuery _:
                        await _dispatcherClient.SendToClient(rawMessage.Sender, FastCall.GetCreator(queryResult)() as IMessage);
                        break;
                }
            }
            finally
            {
                _realTarget = null;
                if (commandHandler != null)
                    commandHandler.Session = null;
            }
        }
    }
}