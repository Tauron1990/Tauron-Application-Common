using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

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

        public Expression Create(ErrorTracer errorTracer, CompilationUnit unit)
        {
            errorTracer.Phase = "Injecting Array for " + _target;

            try
            {
                return Expression.NewArrayInit(_target, _resolvers.Select(r => Expression.Convert(r.Create(errorTracer, unit), _target)));
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