using System;
using System.Collections.Generic;
using System.Linq;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    public class ListResolver : IResolver
    {
        public ListResolver(IEnumerable<IResolver> resolvers, Type target)
        {
            _resolvers = Argument.NotNull(resolvers, nameof(resolvers));
            _target = Argument.NotNull(target, nameof(target));
        }
        
        public object Create(ErrorTracer errorTracer)
        {
            try
            {
                errorTracer.Phase = "Injecting List for " + _target;

                var closed = InjectorBaseConstants.List.MakeGenericType(_target.GenericTypeArguments[0]);
                if (_target.IsAssignableFrom(closed))
                {
                    var info = Argument.CheckResult(closed.GetMethod("Add"), "Add Method For List Required");

                    var args = _resolvers.Select(resolver => resolver.Create(errorTracer)).TakeWhile(vtemp => !errorTracer.Exceptional).ToList();

                    if (errorTracer.Exceptional) return null;

                    var temp = closed.FastCreateInstance();

                    foreach (var o in args) info.InvokeFast(temp, o);

                    return temp;
                }

                errorTracer.Exceptional = true;
                errorTracer.Exception = new InvalidOperationException(_target + " is Not Compatible");

                return null;
            }
            catch (Exception e)
            {
                errorTracer.Exceptional = true;
                errorTracer.Exception = e;
                return null;
            }
        }

        private readonly IEnumerable<IResolver> _resolvers;
        private readonly Type _target;
    }
}