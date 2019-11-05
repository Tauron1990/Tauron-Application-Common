using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tauron.Application.CQRS.Dispatcher.Hubs;

namespace Tauron.Application.CQRS.Dispatcher
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config) => _config = config;

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ServerConfiguration>(c =>
            {
                c.WithDatabase(_config.GetValue<string>("ConnectionString"));
                c.Memory = _config.GetValue<bool>("Memory");
            });

            
            services.AddMvc(o => o.EnableEndpointRouting = false)
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = "simple-scheme";

                // you can also skip this to make the challenge scheme handle the forbid as well
                options.DefaultForbidScheme = "simple-scheme";

                // of course you also need to register that scheme, e.g. using
                options.AddScheme<MySchemeHandler>("simple-scheme", "simple-scheme");
            });
        }

        [UsedImplicitly]
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggingBuilder loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<EventHub>("EventBus", options =>
                                                       {
                                                           options.ApplicationMaxBufferSize = (32768 * 2) * 2;
                                                           options.TransportMaxBufferSize = (32768 * 2) * 2;
                                                           options.Transports = HttpTransportType.WebSockets | HttpTransportType.LongPolling;
                                                       });
                endpoints.MapGet("/", context =>
                {
                    context.Response.Redirect("/Health");

                    return Task.CompletedTask;
                });
            });
        }
    }
}
