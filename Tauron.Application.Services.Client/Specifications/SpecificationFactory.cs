using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Tauron.Application.Services.Client.Specifications
{
    public static class SpecificationFactory<TTarget>
    {
        private static ImmutableDictionary<string, ISpecification> Specifications = ImmutableDictionary<string, ISpecification>.Empty;

        public static ISpecification GetSpecification(Func<ISpecification> factory, string name)
        {
            if (Specifications.TryGetValue(name, out var specification)) return specification;

            specification = factory();

            ImmutableInterlocked.TryAdd(ref Specifications, name, specification);

            return specification;
        }
    }
}