using System;
using System.Linq;
using JetBrains.Annotations;
using Tauron.Application.Ioc.LifeTime;

namespace Tauron.Application.Common.CastleProxy.Aop.Threading
{
    [PublicAPI]
    public interface IBaseHolder
    {
        [NotNull]
        string Name { get; set; }
        
    }
    
    public static class BaseHolder
    {
        [NotNull]
        public static TValue GetOrAdd<TKey, TValue>([NotNull] ObjectContext context, [NotNull] Func<TValue> factory, [NotNull] string name)
            where TKey : class, IBaseHolder where TValue : class, TKey
        {
            Argument.NotNull(context, nameof(context));
            Argument.NotNull(factory, nameof(factory));
            Argument.NotNull(name, nameof(name));

            if (context.GetAll<TKey>().FirstOrDefault(holder => holder.Name == name) is TValue instance) return instance;

            instance = factory();
            context.Register<TKey, TValue>(instance);
            return instance;
        }
    }

    public abstract class BaseHolder<TValue> : IBaseHolder, IDisposable
        where TValue : class
    {
        public void Dispose()
        {
            if (Value is IDisposable dipo) dipo.Dispose();
            GC.SuppressFinalize(this);
        }
        
        protected BaseHolder([NotNull] TValue value) => Value = Argument.NotNull(value, nameof(value));

        ~BaseHolder() => Dispose();

        [NotNull]
        public TValue Value { get; set; }

        public string Name { get; set; }

    }
}