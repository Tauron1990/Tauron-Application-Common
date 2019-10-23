using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Tauron.Application.Ioc;

namespace Tauron.Application.Common.BaseLayer.Data
{
    [PublicAPI]
    public interface IDatabaseAccess : IDisposable
    {
        void SaveChanges();

        T GetRepository<T>()
            where T : class;

        T GetContext<T>();

        Task SaveChangesAsync(CancellationToken sourceToken);

        //ILayerTransaction BeginTransaction();
    }

    //public interface ILayerTransaction : IDisposable
    //{
    //    void Commit();
    //    void Rollback();
    //}

    [Export(typeof(RepositoryFactory))]
    [PublicAPI]
    [DebuggerStepThrough]
    public class RepositoryFactory : INotifyBuildCompled
    {
        private static RepositoryFactory _repositoryFactory;
        private bool _compositeMode;

        private DatabaseDisposer _databaseDisposer;
        
        private Dictionary<Type, (IDatabaseFactory, Type)> _databaseFactories;

        [Inject]
        private List<IRepositoryExtender> _extenders;

        private GroupDictionary<IDatabaseIdentifer, object> _repositorys;

        private object _sync = new object();

        public static RepositoryFactory Factory => _repositoryFactory ?? (_repositoryFactory = CommonApplication.Current.Container.Resolve<RepositoryFactory>());

        void INotifyBuildCompled.BuildCompled()
        {
            _databaseFactories = new Dictionary<Type, (IDatabaseFactory, Type)>();

            foreach (var repositoryExtender in _extenders)
            {
                var fac = repositoryExtender.DatabaseFactory;

                foreach (var repositoryType in repositoryExtender.GetRepositoryTypes()) _databaseFactories.Add(repositoryType.Item1, (fac, repositoryType.Item2));
            }

            _extenders = null;
        }

        public IDatabaseAccess EnterCompositeMode()
        {
            var enter = Enter();
            _compositeMode = true;
            return enter;
        }

        public IDatabaseAccess Enter()
        {
            if (_compositeMode) return new NullDispose(this);
            if (!Monitor.TryEnter(_sync, TimeSpan.FromMinutes(160))) throw new InvalidOperationException("Only One Database Acess Alowed");

            _repositorys = new GroupDictionary<IDatabaseIdentifer, object>();
            _databaseDisposer = new DatabaseDisposer(_repositorys, Exit, this);

            return _databaseDisposer;
        }

        public TRepo GetRepository<TRepo>()
            where TRepo : class => (TRepo) GetRepository(typeof(TRepo));

        public object GetRepository(Type repoType)
        {
            if (!_databaseFactories.TryGetValue(repoType, out var fac))
                throw new InvalidOperationException("No Repository Registrated");

            var dbEnt = _repositorys.FirstOrDefault(p => p.Key.Id == fac.Item1.Id);

            var repo = dbEnt.Value?.FirstOrDefault(obj => obj.GetType() == repoType);

            if (repo != null) return repo;

            var db = dbEnt.Key ?? fac.Item1.CreateDatabase();


            var robj = fac.Item2.FastCreateInstance(db); // Activator.CreateInstance(fac.Item2, db);
            if (dbEnt.Key == null)
                _repositorys.Add(db, robj);

            return robj;
        }

        private void Exit()
        {
            _repositorys = null;
            _databaseDisposer = null;
            _compositeMode = false;
            Monitor.Exit(_sync);
        }

        private class NullDispose : IDatabaseAccess
        {
            private readonly RepositoryFactory _fac;

            public NullDispose(RepositoryFactory fac) => _fac = fac;

            public void Dispose() { }

            public void SaveChanges() { }

            public T GetRepository<T>() where T : class => _fac.GetRepository<T>();

            public T GetContext<T>() => default;

            public Task SaveChangesAsync(CancellationToken sourceToken) => Task.CompletedTask;

            //public ILayerTransaction BeginTransaction() => null;
        }

        private class DatabaseDisposer : IDatabaseAccess
        {
            private readonly GroupDictionary<IDatabaseIdentifer, object> _databases;
            private readonly Action _exitAction;
            private readonly RepositoryFactory _fac;

            public DatabaseDisposer(GroupDictionary<IDatabaseIdentifer, object> databases, Action exitAction, RepositoryFactory fac)
            {
                _databases = databases;
                _exitAction = exitAction;
                _fac = fac;
            }

            public void Dispose()
            {
                foreach (var database in _databases.Keys)
                    database.Dispose();

                _exitAction();
            }

            public void SaveChanges()
            {
                foreach (var database in _databases.Keys.OfType<IDatabase>())
                    database.SaveChanges();
            }

            public T GetRepository<T>() where T : class => _fac.GetRepository<T>();

            public T GetContext<T>() => (T) GetDbContext(typeof(T));

            public Task SaveChangesAsync(CancellationToken sourceToken)
            {
                return Task.WhenAll(_databases.Keys.OfType<IDatabase>().Select(d => d.SaveChangesAsync(sourceToken)));
            }

            //public ILayerTransaction BeginTransaction()
            //{

            //}

            private object GetDbContext(Type dbContext)
            {
                return _databases.Keys.SingleOrDefault(di => di.Context?.GetType() == dbContext)?.Context;
            }
        }
    }
}