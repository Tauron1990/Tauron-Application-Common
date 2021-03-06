﻿using System.Collections.Generic;
using System.Linq;

namespace Tauron.Application.Common.BaseLayer.Data
{
    public class Repository<TEntity, TKey> : IRepository<TEntity, TKey>
        where TEntity : GenericBaseEntity<TKey>
    {
        private readonly IDatabase _database;

        protected TType GetContext<TType>() => (TType) _database.Context;

        public Repository(IDatabase database) => _database = database;

        public IQueryable<TEntity> Query() => _database.Query<TEntity>();

        public IQueryable<TEntity> QueryAsNoTracking() => _database.QueryAsNoTracking<TEntity>();

        public TEntity Find(TKey key) => _database.Find<TEntity, TKey>(key);

        public void Update(TEntity entity) => _database.Update(entity);

        public void Remove(TEntity entity) => _database.Remove(entity);

        public void Add(TEntity entity) => _database.Add(entity);

        public void AddRange(IEnumerable<TEntity> newEntities) => _database.AddRange(newEntities);
    }
}