using System;
using Microsoft.Extensions.DependencyInjection;

namespace Tauron.Application.CQRS.Dispatcher.EventStore
{
    public sealed class ScopeDisposeable<TType> : IDisposable
    {
        public TType Target { get; }

        private readonly IServiceScope _scope;

        public ScopeDisposeable(IServiceScope scope, TType target)
        {
            Target = target;
            _scope = scope;
        }

        public void Dispose() => _scope?.Dispose();

        public static implicit operator TType(ScopeDisposeable<TType> scope)
            => scope.Target;
    }
}