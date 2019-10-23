using Microsoft.Extensions.DependencyInjection;

namespace Tauron.Application.CQRS.Server.Extensions
{
    public static class Extensions
    {
        public static void AddCQRS(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSignalR();
        }
    }
}