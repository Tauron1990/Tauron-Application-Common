using System;
using Microsoft.Extensions.DependencyInjection;

namespace Tauron.CQRS.Services.Core.Components
{
    internal static class AggregateFactory<T>
    {
        private static readonly ObjectFactory Constructor = CreateTypeConstructor();

        private static ObjectFactory CreateTypeConstructor() 
            => ActivatorUtilities.CreateFactory(typeof(T), new Type[0]);

        public static T CreateAggregate(IServiceProvider serviceProvider)
        {
            if (Constructor == null)
            {
                throw new InvalidOperationException("No Constructor For Aggregate");
            }
            return (T)Constructor(serviceProvider, new object[0]);
        }
    }
}