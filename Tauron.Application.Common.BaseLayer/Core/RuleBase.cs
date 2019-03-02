using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FastExpressionCompiler;
using JetBrains.Annotations;
using Tauron.Application.Common.BaseLayer.BusinessLayer;
using Tauron.Application.Common.BaseLayer.Data;

namespace Tauron.Application.Common.BaseLayer.Core
{
    [PublicAPI]
    public abstract class RuleBase : IRuleBase
    {
        private class DbDelegator : IDatabaseAccess
        {
            private IDatabaseAccess _databaseAccessImplementation;
            private Action _dispose;

            public DbDelegator(IDatabaseAccess databaseAccessImplementation, Action dispose)
            {
                _databaseAccessImplementation = databaseAccessImplementation;
                _dispose = dispose;
            }

            public void Dispose()
            {
                _dispose();
                _databaseAccessImplementation.Dispose();
            }

            public void SaveChanges() => _databaseAccessImplementation.SaveChanges();

            public T GetRepository<T>() where T : class => _databaseAccessImplementation.GetRepository<T>();

            public T GetContext<T>() => _databaseAccessImplementation.GetContext<T>();

            public Task SaveChangesAsync(CancellationToken sourceToken) => _databaseAccessImplementation.SaveChangesAsync(sourceToken);
        }

        private static Dictionary<Type, (Action<RuleBase, RepositoryFactory> Init, Action<RuleBase> Dispose)> _enterActions 
            = new Dictionary<Type, (Action<RuleBase, RepositoryFactory>, Action<RuleBase>)>();

        [InjectRepositoryFactory]
        public RepositoryFactory RepositoryFactory { get; set; }

        public abstract bool HasResult { get; }

        public bool Error { get; private set; }
        public IEnumerable<object> Errors { get; private set; }

        public virtual string InitializeMethod { get; }

        public abstract object GenericAction(object input);

        protected internal void SetError([CanBeNull]params object[] errors) => SetError((IEnumerable<object>) errors);

        protected void SetError(IEnumerable<object> objects)
        {
            Error = objects != null;
            Errors = objects != null ? new ReadOnlyEnumerator<object>(objects) : null;
        }

        protected IDatabaseAccess Enter()
        {
            var type = GetType();

            if (!_enterActions.TryGetValue(type, out var action))
            {
                action = CreateInit(type);
                _enterActions[type] = action;
            }

            var enter = RepositoryFactory.Enter();

            action.Init(this, RepositoryFactory);

            return new DbDelegator(enter, () => action.Dispose(this));
        }

        private static (Action<RuleBase, RepositoryFactory>, Action<RuleBase>) CreateInit(Type target)
        {
            var ruleParm = Expression.Parameter(typeof(RuleBase));
            var facParm = Expression.Parameter(typeof(RepositoryFactory));
            List<Expression> block = new List<Expression>();
            List<Expression> disposer = new List<Expression>();

            foreach (var propertyInfo in target.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(pi => pi.IsDefined(typeof(InjectRepoAttribute)) && pi.GetMethod != null && pi.SetMethod != null))
            {
                var castRule = Expression.Convert(ruleParm, target);

                disposer.Add(Expression.Assign(Expression.Property(castRule, propertyInfo.Name), Expression.Constant(null, propertyInfo.PropertyType)));
                Type propertyType = propertyInfo.PropertyType;

                if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Lazy<>))
                {
                    var constructor = Argument.CheckResult(propertyType.GetConstructor(new []{ typeof(Func<>).MakeGenericType(propertyType.GetGenericArguments()[0]) }), 
                        "Constructor not Found"); //.Single(c => c.GetParameters().Length == 1 && c.GetParameters()[0].ParameterType.Name.StartsWith("Func"));

                    block.Add(Expression.Assign(
                        Expression.Property(castRule, propertyInfo), 
                        Expression.New(
                            constructor,
                            Expression.Lambda(
                                constructor.GetParameters()[0].ParameterType,
                                Expression.Call(facParm, nameof(RepositoryFactory.GetRepository), new[]
                                {
                                    constructor.GetParameters()[0].ParameterType.GenericTypeArguments[0]
                                })))));
                }
                else
                {
                    block.Add(Expression.Assign(
                        Expression.Property(castRule, propertyInfo.Name),
                        Expression.Call(facParm, nameof(RepositoryFactory.GetRepository), new[] {propertyType})));
                }
            }

            return (Expression.Lambda<Action<RuleBase, RepositoryFactory>>(Expression.Block(block), ruleParm, facParm).CompileFast(),
                    Expression.Lambda<Action<RuleBase>>(Expression.Block(disposer), ruleParm).CompileFast());
        }
    }
}