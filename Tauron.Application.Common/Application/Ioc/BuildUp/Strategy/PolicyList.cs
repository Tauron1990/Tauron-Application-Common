using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Tauron.Application.Ioc.BuildUp.Strategy
{
    [PublicAPI]
    public class PolicyList
    {
        private readonly GroupDictionary<Type, IPolicy> _list = new GroupDictionary<Type, IPolicy>();

        public void Add<TPolicy>([NotNull] TPolicy policy) where TPolicy : IPolicy => _list.Add(typeof(TPolicy), Argument.NotNull(policy, nameof(policy)));

        public TPolicy Get<TPolicy>()
        {
            if (!_list.TryGetValue(typeof(TPolicy), out var policies)) return default;
            return (TPolicy) policies.Last();
        }

        public IEnumerable<TPolicy> GetAll<TPolicy>() => !_list.TryGetValue(typeof(TPolicy), out var policies) ? Enumerable.Empty<TPolicy>() : policies.Cast<TPolicy>();

        public void Remove<TPolicy>() => _list.Remove(typeof(TPolicy));

        public void Remove<TPolicy>(TPolicy policy) where TPolicy : IPolicy => _list.RemoveValue(policy);
    }
}