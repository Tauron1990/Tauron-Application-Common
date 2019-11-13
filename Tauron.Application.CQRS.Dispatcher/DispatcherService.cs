using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Tauron.Application.CQRS.Common.Server;
using Tauron.Application.CQRS.Dispatcher.Core;
using Tauron.Application.CQRS.Dispatcher.EventStore;

namespace Tauron.Application.CQRS.Dispatcher
{
    [UsedImplicitly]
    public class DispatcherService : BackgroundService
    {
        private readonly IEventManager _eventManager;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IOptions<ServerConfiguration> _config;
        private readonly ILogger<DispatcherService> _logger;

        private CancellationTokenRegistration _tokenRegistration;
        private Task? _runningTask;

        public DispatcherService(IEventManager eventManager, ILogger<DispatcherService> logger, IServiceScopeFactory scopeFactory, IOptions<ServerConfiguration> config)
        {
            _eventManager = eventManager;
            _scopeFactory = scopeFactory;
            _config = config;
            _logger = logger;
        }

        #region Overrides of BackgroundService

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_config.Value.Memory)
            {
                using var prov = _scopeFactory.CreateScope();
                using var context = prov.ServiceProvider.GetRequiredService<DispatcherDatabaseContext>();
                context.Database.Migrate();
            }


            _runningTask = Task.Run(_eventManager.StartMessageQueue);
            _tokenRegistration = stoppingToken.Register(_eventManager.StopMessageQueue);

            return Task.CompletedTask;
        }

        #endregion

        public override void Dispose()
        {
            base.Dispose();
            _tokenRegistration.Dispose();
            _runningTask?.Dispose();
        }
    }
}