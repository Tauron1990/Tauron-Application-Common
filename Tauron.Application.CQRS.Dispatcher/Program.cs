using Karambolo.Extensions.Logging.File;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Tauron.Application.CQRS.Dispatcher
{
    public static class Program
    {
        public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.ConfigureLogging((ctx, builder) =>
                    {
                        builder.AddConfiguration(ctx.Configuration.GetSection("Logging"));
                        builder.AddFile(o =>
                        {
                            o.RootPath = ctx.HostingEnvironment.ContentRootPath;
                            o.MaxFileSize = 100 * 1024 * 1024;
                            o.Files = new[] { new LogFileOptions() };
                        });
                    })
                    .UseStartup<Startup>()
                    .UseIIS());
    }
}
