using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Options;

namespace Tauron.Application.CQRS.Dispatcher.EventStore
{
    [UsedImplicitly]
    public class DesignTimeContext : IDesignTimeDbContextFactory<DispatcherDatabaseContext>
    {
        public DispatcherDatabaseContext CreateDbContext(string[] args) 
            => new DispatcherDatabaseContext(new OptionsWrapper<ServerConfiguration>(new ServerConfiguration().WithDatabase("Data Source=localhost\\SQLEXPRESS;Initial Catalog=Dispatcher;Integrated Security=True;")));
    }
}