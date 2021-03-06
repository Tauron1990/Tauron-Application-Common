﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tauron.Application.CQRS.Client.Domain;
using Tauron.Application.CQRS.Common.Dto;
using Tauron.Application.CQRS.Common.Server;

namespace Tauron.Application.CQRS.Client.Core.Components.Handler
{
    public class HandlerListDelegator
    {
        private readonly List<Func<HandlerBase>> _handlers;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public HandlerListDelegator(List<Func<HandlerBase>> handlers, IServiceScopeFactory serviceScopeFactory)
        {
            _handlers = handlers;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task Handle(IMessage? msg, DomainMessage rawMessage)
        {
            if (msg == null) return;

            using var scope = _serviceScopeFactory.CreateScope();

            var handler = (GlobalEventHandlerBase) scope.ServiceProvider.GetRequiredService(typeof(GlobalEventHandler<>).MakeGenericType(msg.GetType()));

            await handler.Handle(msg);

            var session = scope.ServiceProvider.GetRequiredService<ISession>();
            try
            {
                foreach (var handlerInstace in _handlers)
                    await handlerInstace().Handle(msg, rawMessage);
            }
            finally
            {
                await ((IInternalSession) session).Commit();
            }
        }
    }
}