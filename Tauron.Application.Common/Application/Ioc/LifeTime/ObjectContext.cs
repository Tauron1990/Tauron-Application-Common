using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Tauron.Application.Ioc.Components;

namespace Tauron.Application.Ioc.LifeTime
{
    [PublicAPI]
    public sealed class ObjectContext : IDisposable
    {
        private readonly ComponentRegistry _registry;

        public ObjectContext() => _registry = new ComponentRegistry();

        public void Dispose()
        {
            _registry.Dispose();
        }
        public void DisposeAll()
        {
            lock (_registry)
                _registry.Dispose();
        }


        public TInterface Get<TInterface>() where TInterface : class => _registry.Get<TInterface>();

        public IEnumerable<TInterface> GetAll<TInterface>() where TInterface : class => _registry.GetAll<TInterface>();

        public void Register<TInterface, TImplement>() where TImplement : TInterface, new() => _registry.Register<TInterface, TImplement>();

        public void Register<TInterface, TImplement>([NotNull] TImplement instance)
            where TImplement : TInterface
        {
            Argument.NotNull(instance, nameof(instance));
            _registry.Register<TInterface, TImplement>(instance);
        }
    }
}