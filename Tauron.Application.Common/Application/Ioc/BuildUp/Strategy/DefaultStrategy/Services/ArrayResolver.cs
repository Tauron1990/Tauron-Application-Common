﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DefaultStrategy
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

        public object Create(ErrorTracer errorTracer)
        {
            errorTracer.Phase = "Injecting Array for " + _target;

            try
            {
                var arr = Array.CreateInstance(_target, _resolvers.Length);

                var index = 0;

                foreach (var val in _resolvers.Select(resolver => resolver.Create(errorTracer)))
                {
                    if (errorTracer.Exceptional) return arr;

                    arr.SetValue(val, index);
                    index++;
                }

                return arr;
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