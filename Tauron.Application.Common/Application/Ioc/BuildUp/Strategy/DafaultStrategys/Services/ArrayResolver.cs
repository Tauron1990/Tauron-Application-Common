using System;
using System.Collections.Generic;
using System.Linq;
using ExpressionBuilder;
using ExpressionBuilder.Fluent;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    public sealed class ArrayResolver : IResolver
    {
        private readonly IResolver[] _resolvers;
        private readonly Type _target;

        public ArrayResolver(IEnumerable<IResolver> resolvers, Type target)
        {
            _resolvers = resolvers.ToArray();
            _target = target;
        }

        public IRightable Create(ErrorTracer errorTracer)
        {
            errorTracer.Phase = "Injecting Array for " + _target;

            try
            {
                return Operation.CreateArray(_target, _resolvers.Select(r => r.Create(errorTracer)).ToArray());
            }
            catch (Exception e)
            {
                errorTracer.Exceptional = true;
                errorTracer.Exception = e;
                return null;
            }
        }
    }
}