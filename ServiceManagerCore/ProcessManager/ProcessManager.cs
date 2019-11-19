using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nito.AsyncEx;
using ServiceManager.Core.Core;
using ServiceManager.Core.Services;
using Tauron.Application.CQRS.Client.Commands;
using Tauron.Application.CQRS.Extensions.ServiceControl;

namespace ServiceManager.Core.ProcessManager
{
    public class ProcessManager : IProcessManager, IDisposable
    {
        private class ProcessHolder : IDisposable
        {
            private readonly AsyncLock _lock = new AsyncLock();
            private readonly Process _process;

            public ProcessHolder(Process process)
            {
                _process = process;
            }

            public async Task<TType> Execute<TType>(Func<Process, Task<TType>> runner)
            {
                using (await _lock.LockAsync()) 
                    return await runner(_process);
            }

            public void Dispose() => _process?.Dispose();
        }

        private readonly ILogger<ProcessManager> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ServiceSettings _serviceSettings;
        private readonly IOptions<ServiceManagerConfiguration> _config;
        private readonly ConcurrentDictionary<string, ProcessHolder> _processes = new ConcurrentDictionary<string, ProcessHolder>();

        public ProcessManager(ILogger<ProcessManager> logger, IServiceScopeFactory serviceScopeFactory, ServiceSettings serviceSettings, IOptions<ServiceManagerConfiguration> config)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _serviceSettings = serviceSettings;
            _config = config;
        }

        public Task<bool> Start(RunningService service)
        {
            if(_processes.ContainsKey(service.Name)) return Task.FromResult(false);

            try
            {
                var process = Process.Start(Path.Combine(service.InstallationPath, service.Exe));
                process.EnableRaisingEvents = true;

                _processes[service.Name] = new ProcessHolder(process);

                service.ServiceStade = ServiceStade.Running;

                process.Exited += (sender, args) =>
                {
                    service.ServiceStade = ServiceStade.Ready;
                    if (_processes.TryRemove(service.Name, out var holder)) 
                        holder.Dispose();
                };

                _logger.LogInformation($"Service Started: {service.Name}");

                return Task.FromResult(true);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"{service.Name}: Error on Start Process");

                service.ServiceStade = ServiceStade.Error;
                return Task.FromResult(false);
            }
        }

        public async Task<bool> Stop(RunningService service, int timeToKill)
        {
            if (_processes.TryGetValue(service.Name, out var process))
            {
                return await process.Execute(async p =>
                {
                    using var scope = _serviceScopeFactory.CreateScope();

                    var error = false;

                    try
                    {
                        using var waiter = new ServiceStopWaiter(service.Name);
                        await scope.ServiceProvider.GetRequiredService<ICommandSender>().Send(new StopServiceCommand {Name = service.Name}).ConfigureAwait(false);

                        await waiter.Wait(10_000);
                        if (p.WaitForExit(10_000))
                            return true;
                        else
                        {
                            p.Kill();
                            return true;
                        }
                    }
                    catch (InvalidOperationException e)
                    {
                        error = true;

                        _logger.LogError(e, $"{service.Name}: Invalid process Handle");
                        _processes.TryRemove(service.Name, out _);
                        return true;
                    }
                    catch (Exception e)
                    {
                        error = true;

                        _logger.LogError(e, $"{service.Name}: Error on Stop");
                        return false;
                    }
                    finally
                    {
                        service.ServiceStade = error ? ServiceStade.Error : ServiceStade.Ready;
                    }
                });
            }

            return false;
        }

        public async Task StartAll()
        {
            foreach (var service in _serviceSettings.RunningServices) await Start(service);
        }

        public async Task StopAll()
        {
            foreach (var service in _serviceSettings.RunningServices) 
                await Stop(service, 10_000);

            await ServiceSettings.Write(_serviceSettings, _config.Value.SettingsPath);
        }

        public void Dispose()
        {
            foreach (var process in _processes) 
                process.Value.Dispose();
        }
    }
}