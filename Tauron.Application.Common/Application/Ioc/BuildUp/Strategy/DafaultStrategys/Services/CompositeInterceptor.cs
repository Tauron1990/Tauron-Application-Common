using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    public class CompositeInterceptor : IImportInterceptor
    {
        private readonly List<IImportInterceptor> _interceptors;

        public CompositeInterceptor([NotNull] [ItemNotNull] List<IImportInterceptor> interceptors) => _interceptors = interceptors ?? throw new ArgumentNullException(nameof(interceptors));

        public (Expression IsOK, Expression Operation) Intercept(MemberInfo member, ImportMetadata metadata, string targetVariable)
        {
            List<(Expression IsOK, Expression Operation)> resultList = _interceptors.ConvertAll(input => input.Intercept(member, metadata, targetVariable));

            Expression curr = null;
            List<Expression> block = new List<Expression>();

            foreach (var valueTuple in resultList)
            {
                if (curr == null)
                    curr = valueTuple.IsOK;
                else
                    curr = Expression.AndAlso(curr, valueTuple.IsOK);
                block.Add(valueTuple.Operation);
            }

            if(curr == null)
                curr = Expression.Constant(true);

            return (curr, Expression.Block(block));
        }
    }
}