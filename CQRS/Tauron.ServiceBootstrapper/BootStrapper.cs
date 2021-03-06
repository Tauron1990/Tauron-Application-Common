﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Tauron.Application.CQRS.Client;
using Tauron.Application.CQRS.Common;
using Tauron.Application.CQRS.Common.Configuration;
using Tauron.ServiceBootstrapper.Core;
using Tauron.ServiceBootstrapper.Jobs;

namespace Tauron.ServiceBootstrapper
{
    [PublicAPI]
    public static class BootStrapper
    {
        private static readonly ManualResetEvent ExitWaiter = new ManualResetEvent(false);
        private static IServiceProvider? _serviceProvider;

        public static event Func<IServiceProvider, Task>? ShutdownEvent; 

        //public static Task Run(
        //    string[] args, Action<ClientCofiguration> clientConfig = null,
        //    Func<ServiceCollection, Task> config = null,
        //    Func<IServiceProvider, StartOptions<EmtyOptions>, Task> startUp = null) =>
        //    Run<EmtyOptions>(args, clientConfig, config, startUp);

        public static async Task Run<TStartOptions>(
            string[] args, Action<ClientCofiguration>? clientConfig = null,
            Func<ServiceCollection, Task>? config = null, 
            Func<IServiceProvider, StartOptions<TStartOptions>, Task>? startUp = null)
        where TStartOptions : class
        {
            var rootPath = AppContext.BaseDirectory;
            var collection = new ServiceCollection();
            collection.AddSingleton<IJobManager, JobManager>();

            collection.AddLogging(lb =>
            {
                lb.AddConsole();
                lb.AddProvider(new SendingProvider(GetScopeFactory));
                lb.AddFile(o => o.BasePath = rootPath);
            });
            collection.AddOptions();
            var configuration = GetAppConfig(rootPath);
            if (config != null) 
                collection.AddSingleton(configuration);

            foreach (var service in 
                from type in typeof(TStartOptions).Assembly.GetTypes()
                where type.IsDefined(typeof(ServiceAttribute))
                let lifeTime = type.GetCustomAttribute<ServiceAttribute>()
                select new {ServiceType = type, Attribute = lifeTime})
            {
                var descriptor = new ServiceDescriptor(
                    service.Attribute.Interface ?? service.ServiceType, 
                    service.ServiceType, 
                    service.Attribute.Lifetime);

                collection.TryAdd(descriptor);
            }

            var serviceConfig = GetServiceConfig(rootPath);
            string serviceName = serviceConfig.GetValue<string>("ServiceName");
            Console.Title = $"Service Name: {serviceName}";

            collection.AddCqrs(cofiguration =>
            {
                cofiguration.SetUrls(
                    new Uri(serviceConfig.GetValue<string>("Dispatcher"), UriKind.RelativeOrAbsolute),
                    serviceName,
                    serviceConfig.GetValue<string>("ApiKey"))
                    .AddFrom<ServiceStopHandler>()
                    .AddFrom<TStartOptions>();

                clientConfig?.Invoke(cofiguration);
            });

            if(config != null)
                await config(collection);

            JobStore.SearchAt<TStartOptions>();

            var provider = collection.BuildServiceProvider();
            await provider.StartCqrs();
            provider.GetRequiredService<IJobManager>().Start();

            _serviceProvider = provider;
            if (startUp != null)
                await startUp(provider, new StartOptions<TStartOptions>(args));

            Console.WriteLine("Service start Compled");
            ExitWaiter.WaitOne();

            provider.GetRequiredService<IJobManager>().Stop();
            await _serviceProvider.StopCqrs();
            ExitWaiter.Dispose();
            provider.Dispose();
        }

        private static IServiceScopeFactory? GetScopeFactory() => _serviceProvider?.GetService<IServiceScopeFactory>();

        private static IConfiguration GetServiceConfig(string path)
        {
            var realPath = Path.Combine(path, "ServiceSettings.json");
            var configBuilder = new ConfigurationBuilder()
                .AddJsonFile(realPath);

            return configBuilder.Build();
        }

        private static IConfiguration? GetAppConfig(string path)
        {
            var realPath = Path.Combine(path, "AppSettings.json");
            if (!File.Exists(realPath)) return null;

            var configBuilder = new ConfigurationBuilder()
                .AddJsonFile(realPath);

            return configBuilder.Build();
        }

        internal static async Task Shutdown()
        {
            if (ShutdownEvent != null)
                await ShutdownEvent(Guard.CheckNull(_serviceProvider));
            ExitWaiter.Set();
        }
    }
}
