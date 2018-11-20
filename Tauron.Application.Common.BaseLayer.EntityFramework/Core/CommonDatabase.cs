using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Tauron.Application.Common.BaseLayer.Data;

namespace Tauron.Application.Common.BaseLayer.Core
{
    [PublicAPI]
    public class CommonDatabase<TDbContext> : IDatabase
        where TDbContext : DbContext, new()
    {
        public CommonDatabase(TDbContext context) => InternalContext = context;

        protected DbContext InternalContext;

        public void Dispose() => InternalContext.Dispose();

        public virtual string Id { get; } = "Common";
        public object Context => InternalContext;

        public void Remove<TEntity>(TEntity entity)
            where TEntity : BaseEntity => InternalContext.Remove(entity);

        public void Update<TEntity>(TEntity entity)
            where TEntity : BaseEntity => InternalContext.Update(entity);

        public IQueryable<TEntity> Query<TEntity>()
            where TEntity : BaseEntity => InternalContext.Set<TEntity>();

        public IQueryable<TEntity> QueryAsNoTracking<TEntity>() where TEntity : BaseEntity => Query<TEntity>().AsNoTracking();

        public void Add<TEntity>(TEntity entity) where TEntity : BaseEntity => InternalContext.Add(entity);

        public void SaveChanges() => InternalContext.SaveChanges();

        public TEntity Find<TEntity, TKey>(TKey key) where TEntity : GenericBaseEntity<TKey> => InternalContext.Find<TEntity>(key);

        public Task SaveChangesAsync(CancellationToken cancellationToken) => InternalContext.SaveChangesAsync(cancellationToken);

        public void AddRange<TEntity>(IEnumerable<TEntity> newEntities)
        {
            foreach (var newEntity in newEntities)
                InternalContext.Add((object)newEntity);
        }
    }
}