using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Tauron.Application.Ioc.Components
{
    [PublicAPI]
    public sealed class ComponentRegistry : IDisposable, IServiceProvider
    {
        private readonly GroupDictionary<Type, LazyLoad> _dictionary = new GroupDictionary<Type, LazyLoad>();

        private class LazyLoad : IDisposable
        {
            public LazyLoad([NotNull] Type implement, [NotNull] ComponentRegistry registry, object instance)
            {
                _implement = implement;
                _registry = registry;
                _object = instance;
            }

            public object Object
            {
                get
                {
                    lock (this)
                    {
                        if (_isInitialized) return _object;
                        if (_object == null) _object = Activator.CreateInstance(_implement);
                        if (_object is IInitializeable init) init.Initialize(_registry);

                        _isInitialized = true;
                    }

                    return _object;
                }
            }

            public void Dispose()
            {
                if (_object is IDisposable disposable) disposable.Dispose();
            }

            public override string ToString() => _implement.ToString();

            private readonly Type _implement;
            private readonly ComponentRegistry _registry;
            private bool _isInitialized;
            private object _object;

        }

        public void Dispose()
        {
            foreach (var value in _dictionary.AllValues) value.Dispose();

            _dictionary.Clear();
        }

        [System.Diagnostics.Contracts.Pure]
        public TInterface Get<TInterface>() where TInterface : class
        {
            lock (_dictionary)
            {
                var type = typeof(TInterface);
                if (_dictionary.TryGetValue(type, out var list))
                    return (TInterface) list.Single().Object;
            }

            throw new KeyNotFoundException();
        }

        public IEnumerable<TInterface> GetAll<TInterface>() where TInterface : class
        {
            lock (_dictionary)
            {
                var type = typeof(TInterface);
                if (!_dictionary.TryGetValue(type, out var list))
                    yield break;

                foreach (var lazyLoad in list)
                    yield return (TInterface) lazyLoad.Object;
            }
        }

        public void Register<TInterface, TImplement>() where TImplement : TInterface, new()
        {
            lock (_dictionary)
                _dictionary[typeof(TInterface)].Add(new LazyLoad(typeof(TImplement), this, null));
        }

        public void Register<TInterface, TImplement>(bool single) where TImplement : TInterface, new()
        {
            lock (_dictionary)
            {
                if (single)
                {
                    var temp = _dictionary[typeof(TInterface)];
                    temp.Clear();
                    temp.Add(new LazyLoad(typeof(TImplement), this, null));
                    return;
                }

                _dictionary[typeof(TInterface)].Add(new LazyLoad(typeof(TImplement), this, null));
            }
        }

        public void Register<TInterface, TImplementation>([NotNull] TImplementation instance)
                where TImplementation : TInterface
        {
            Argument.NotNull(instance, nameof(instance));

            lock (_dictionary)
                _dictionary[typeof(TInterface)].Add(new LazyLoad(typeof(TImplementation), this, instance));
        }

        public void Register<TInterface, TImplementaion>([NotNull] TImplementaion instance, bool isSingle)
            where TImplementaion : TInterface
        {
            Argument.NotNull(instance, nameof(instance));

            lock (_dictionary)
            {
                if (isSingle)
                {
                    var temp = _dictionary[typeof(TInterface)];
                    temp.Clear();
                    temp.Add(new LazyLoad(typeof(TImplementaion), this, instance));
                }

                _dictionary[typeof(TInterface)].Add(new LazyLoad(typeof(TImplementaion), this, instance));
            }
        }

        object IServiceProvider.GetService(Type serviceType)
        {
            lock (_dictionary)
            {
                var type = serviceType;
                if (_dictionary.TryGetValue(type, out var list))
                    return list.FirstOrDefault()?.Object;
            }

            return null;
        }
    }
}