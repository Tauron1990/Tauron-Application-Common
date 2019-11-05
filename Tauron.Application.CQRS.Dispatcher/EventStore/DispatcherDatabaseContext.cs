using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using Tauron.Application.CQRS.Dispatcher.EventStore.Data;

namespace Tauron.Application.CQRS.Dispatcher.EventStore
{
    public class DispatcherDatabaseContext : DbContext
    {
        private static readonly Lazy<InMemoryDatabaseRoot> DatabaseRoot = new Lazy<InMemoryDatabaseRoot>(() => new InMemoryDatabaseRoot());

        private readonly IOptions<ServerConfiguration> _serverOptions;

        public DbSet<EventEntity> EventEntities { get; set; }

        public DbSet<ApiKey> ApiKeys { get; set; }

        public DbSet<ObjectStadeEntity> ObjectStades { get; set; }

        public DispatcherDatabaseContext(IOptions<ServerConfiguration> serverOptions) 
            => _serverOptions = serverOptions;

        #region Overrides of DbContext

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_serverOptions.Value.Memory)
                optionsBuilder.UseInMemoryDatabase(_serverOptions.Value.ConnectionString, DatabaseRoot.Value);
            else
                optionsBuilder.UseSqlServer(_serverOptions.Value.ConnectionString, builder => builder.EnableRetryOnFailure());
            base.OnConfiguring(optionsBuilder);
        }

        #endregion
    }
}