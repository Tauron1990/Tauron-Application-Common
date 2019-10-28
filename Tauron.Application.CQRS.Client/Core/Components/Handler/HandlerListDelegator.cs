using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tauron.Application.CQRS.Client.Infrastructure;
using Tauron.Application.CQRS.Common.Server;

namespace Tauron.Application.CQRS.Client.Core.Components.Handler
{
    public class HandlerListDelegator
    {
        private readonly List<HandlerBase> _handlers;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public HandlerListDelegator(List<HandlerBase> handlers, IServiceScopeFactory serviceScopeFactory)
        {
            _handlers = handlers;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task Handle(IMessage msg, DomainMessage rawMessage)
        {
            if (rawMessage.EventType == EventType.QueryResult)
            {
                using var scope = _serviceScopeFactory.CreateScope();

                var handler = (GlobalEventHandlerBase)scope.ServiceProvider.GetRequiredService(typeof(GlobalEventHandler<>).MakeGenericType(msg.GetType()));

                await handler.Handle(msg);
            }
            else
            {
                foreach (var handlerInstace in _handlers)
                    await handlerInstace.Handle(msg, rawMessage);
            }
        }
    }
}