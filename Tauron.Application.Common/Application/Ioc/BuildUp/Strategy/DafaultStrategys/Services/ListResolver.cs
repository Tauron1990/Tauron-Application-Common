using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ExpressionBuilder;
using ExpressionBuilder.Fluent;
using ExpressionBuilder.Parser;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    public class ListResolver : IResolver
    {
        private class ListRightable : IRightable
        {
            private readonly IOperation _listOperation;
            private readonly IEnumerable<ICodeLine> _adder;
            private readonly string _variableName;

            public ListRightable(Type lisType, IOperation listOperation, IEnumerable<ICodeLine> adder, string variableName)
            {
                ParsedType = lisType;
                _listOperation = listOperation;
                _adder = adder;
                _variableName = variableName;
            }

            public Type ParsedType { get; }

            public string ToString(ParseContext context)
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine($"{context.Pad} () => {{");
                context.AddLevel();
                builder.AppendLine($"{context.Pad} {_variableName} = {_listOperation.ToString(context)}");

                foreach (var codeLine in _adder)
                    builder.AppendLine(codeLine.ToString(context));

                context.RemoveLevel();
                builder.AppendLine($"{context.Pad} }};");

                return builder.ToString();
            }

            public Expression ToExpression(ParseContext context)
            {
                var variable = Expression.Parameter(ParsedType, _variableName);
                var returnLabel = Expression.Label(ParsedType, "return");
                var returnExpression = Expression.Label(returnLabel);

                List<Expression> exps = new List<Expression> { Expression.Assign(variable, _listOperation.ToExpression(context)) };

                exps.AddRange(_adder.Select(o => o.ToExpression(context)));
                exps.Add(Expression.Return(returnLabel, variable));
                exps.Add(returnExpression);

                return Expression.Lambda(Expression.Block(new[] { variable }, exps.ToArray()));
            }

            public void PreParseExpression(ParseContext context)
            {
                _listOperation.PreParseExpression(context);
                foreach (var operation in _adder) operation.PreParseExpression(context);
            }
        }

        public ListResolver(IEnumerable<IResolver> resolvers, Type target)
        {
            _resolvers = Argument.NotNull(resolvers, nameof(resolvers));
            _target = Argument.NotNull(target, nameof(target));
        }
        
        public IRightable Create(ErrorTracer errorTracer)
        {
            try
            {
                errorTracer.Phase = "Injecting List for " + _target;

                var closed = InjectorBaseConstants.List.MakeGenericType(_target.GenericTypeArguments[0]);
                if (_target.IsAssignableFrom(closed))
                {
                    var info = Argument.CheckResult(closed.GetMethod("Add"), "Add Method For List Required");

                    string variable = "ListVariable";

                    var args = _resolvers.Select(resolver => Operation.Invoke(Operation.Variable(variable), info, resolver.Create(errorTracer)))
                        .TakeWhile(vtemp => !errorTracer.Exceptional).ToList();

                    if (errorTracer.Exceptional) return null;

                    var temp = Operation.CreateInstance(closed);
                    
                    return new ListRightable(closed, temp, args, variable);
                }

                errorTracer.Exceptional = true;
                errorTracer.Exception = new InvalidOperationException(_target + " is Not Compatible");

                return Operation.Null();
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