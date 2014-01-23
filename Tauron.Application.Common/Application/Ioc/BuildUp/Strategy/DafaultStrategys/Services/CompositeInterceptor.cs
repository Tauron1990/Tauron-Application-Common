using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reflection;
using Tauron.Application.Ioc.BuildUp.Exports;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    public class CompositeInterceptor : IImportInterceptor
    {
        private readonly List<IImportInterceptor> _interceptors;

        public CompositeInterceptor([NotNull] List<IImportInterceptor> interceptors)
        {
            Contract.Requires<ArgumentNullException>(interceptors != null, "interceptors");
            Contract.Requires<ArgumentOutOfRangeException>(
                Contract.ForAll(interceptors, interceptor => interceptor != null), "interceptors");

            _interceptors = interceptors;
        }

        public bool Intercept(MemberInfo member, ImportMetadata metadata, object target, ref object value)
        {
            bool returnValue = true;

            foreach (var importInterceptor in _interceptors)
            {
                if (returnValue) returnValue = importInterceptor.Intercept(member, metadata, target, ref value);
            }

            return returnValue;
        }
    }
}
