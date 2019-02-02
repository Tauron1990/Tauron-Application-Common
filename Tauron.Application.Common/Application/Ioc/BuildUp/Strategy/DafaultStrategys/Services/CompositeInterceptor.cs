using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExpressionBuilder;
using ExpressionBuilder.Fluent;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    public class CompositeInterceptor : IImportInterceptor
    {
        private readonly List<IImportInterceptor> _interceptors;

        public CompositeInterceptor([NotNull] [ItemNotNull] List<IImportInterceptor> interceptors) => _interceptors = interceptors ?? throw new ArgumentNullException(nameof(interceptors));

        public (Condition IsOK, ICodeLine Operation) Intercept(MemberInfo member, ImportMetadata metadata, string targetVariable)
        {
            List<(Condition IsOK, ICodeLine Operation)> resultList = _interceptors.ConvertAll(input => input.Intercept(member, metadata, targetVariable));

            return (Condition.And(resultList.Select(vt => vt.IsOK).ToArray()), 
                Operation.NeestedLambda("Interceptor", typeof(void), parameter => { parameter.WithBody(resultList.Select(sel => sel.Operation)); }));
        }
    }
}