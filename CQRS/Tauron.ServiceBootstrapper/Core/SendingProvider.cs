﻿using System;
using System.Threading;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tauron.Application.CQRS.Client.Events;
using Tauron.Application.CQRS.Common;
using Tauron.Application.CQRS.Common.Configuration;
using Tauron.Application.CQRS.Extensions.ServiceControl.Logging;

namespace Tauron.ServiceBootstrapper.Core
{
    public class SendingProvider : ILoggerProvider
    {
        private class Logger : ILogger
        {
            private class ActionDispose : IDisposable
            {
                private readonly Action _action;

                public ActionDispose(Action action) => _action = action;

                public void Dispose() => _action();
            }

            private readonly string _category;
            private readonly Func<(IEventPublisher, ClientCofiguration)> _factory;
            private int _scope;

            public Logger(string category, Func<(IEventPublisher, ClientCofiguration)> factory)
            {
                _category = category;
                _factory = factory;
            }

            public async void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                var (sender, config) = _factory();
                if(sender == null) return;

                await sender.Publish(new LoggingEvent(_category, logLevel, eventId, formatter(state, exception), _scope, config.ServiceName));
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return true;
            }

            public IDisposable BeginScope<TState>(TState state)
            {
                Interlocked.Increment(ref _scope);
                return new ActionDispose(() => Interlocked.Decrement(ref _scope));
            }
        }

        private Func<IServiceScopeFactory?>? _factory;

        private IEventPublisher? _eventPublisher;
        private ClientCofiguration? _clientCofiguration;

        public SendingProvider([NotNull]Func<IServiceScopeFactory?> factory) => _factory = factory;

        private (IEventPublisher, ClientCofiguration) GetEventPublisher()
        {
            lock (this)
            {
                if (_eventPublisher != null && _clientCofiguration != null) return (_eventPublisher, _clientCofiguration);

                var temp = _factory?.Invoke();
                if (temp == null) return default;

                using var scope = temp.CreateScope();
                _eventPublisher = scope.ServiceProvider.GetService<IEventPublisher>();
                if (_eventPublisher != null)
                    _factory = null;

                return (Guard.CheckNull(_eventPublisher), Guard.CheckNull(_clientCofiguration = scope.ServiceProvider.GetRequiredService<IOptions<ClientCofiguration>>().Value));
            }
        }

        public void Dispose()
        {
            _eventPublisher = null;
            _factory = null;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new Logger(categoryName, GetEventPublisher);
        }
    }
}