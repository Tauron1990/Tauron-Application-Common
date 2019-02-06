using System;
using System.Collections.Generic;
using System.Linq;
using ExpressionBuilder;
using ExpressionBuilder.Fluent;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    public class ListResolver : IResolver
    {
        //private class ListRightable : IRightable
        //{
        //    private readonly IOperation _listOperation;
        //    private readonly IEnumerable<ICodeLine> _adder;
        //    private readonly string _variableName;
        //    private readonly Type _listType;

        //    public ListRightable(Type listType, IOperation listOperation, IEnumerable<ICodeLine> adder, string variableName)
        //    {
        //        _listType = listType;
        //        _listOperation = listOperation;
        //        _adder = adder;
        //        _variableName = variableName;
        //        ParsedType = typeof(Func<>).MakeGenericType(_listType);
        //    }

        //    public Type ParsedType { get; }

        //    public string ToString(ParseContext context)
        //    {
        //        StringBuilder builder = new StringBuilder();
        //        builder.AppendLine($"{context.Pad} () => {{");
        //        context.AddLevel();
        //        builder.AppendLine($"{context.Pad} {_variableName} = {_listOperation.ToString(context)}");

        //        foreach (var codeLine in _adder)
        //            builder.AppendLine(codeLine.ToString(context));

        //        context.RemoveLevel();
        //        builder.AppendLine($"{context.Pad} }};");

        //        return builder.ToString();
        //    }

        //    public Expression ToExpression(ParseContext context)
        //    {
        //        context.AddLevel();
        //        var variable = Expression.Parameter(_listType, _variableName);
        //        context.Current.AddVariable(new Variable(_listType, _variableName));
        //        //var returnLabel = Expression.Label(_listType, "return");
        //        //var returnExpression = Expression.Label(returnLabel, Expression.Default(_listType));

        //        List<Expression> exps = new List<Expression> { Expression.Assign(variable, _listOperation.ToExpression(context)) };

        //        exps.AddRange(_adder.Select(o => o.ToExpression(context)));
        //        //exps.Add(Expression.Return(returnLabel, variable));
        //        //exps.Add(returnExpression);
        //        exps.Add(variable);

        //        context.RemoveLevel();

        //        return Expression.Lambda(Expression.Block(new[] {variable}, exps.ToArray()));
        //    }

        //    public void PreParseExpression(ParseContext context)
        //    {
        //        context.AddLevel();
        //        context.Current.AddVariable(new Variable(_listType, _variableName));
        //        _listOperation.PreParseExpression(context);
        //        foreach (var operation in _adder) operation.PreParseExpression(context);
        //        context.RemoveLevel();
        //    }
        //}

        public ListResolver(IEnumerable<IResolver> resolvers, Type target)
        {
            _resolvers = Argument.NotNull(resolvers, nameof(resolvers));
            _target = Argument.NotNull(target, nameof(target));
        }
        
        public IRightable Create(ErrorTracer errorTracer, CompilationUnit unit)
        {
            try
            {
                errorTracer.Phase = "Injecting List for " + _target;

                var targetType = _target.GenericTypeArguments[0];
                var closed = InjectorBaseConstants.List.MakeGenericType(_target.GenericTypeArguments[0]);
                if (_target.IsAssignableFrom(closed))
                {
                    var info = Argument.CheckResult(closed.GetMethod("Add"), "Add Method For List Required");

                    string variable = unit.VariableNamer.GetRandomVariable();

                    var args = _resolvers.Select(resolver => Operation.Invoke(Operation.Variable(variable), info, Operation.Cast(resolver.Create(errorTracer, unit), targetType)))
                        .TakeWhile(vtemp => !errorTracer.Exceptional).ToList();

                    if (errorTracer.Exceptional) return null;

                    var temp = Operation.CreateInstance(closed);

                    var block = Operation.Block(parameter =>
                    {
                        parameter.ReturnVar(variable);
                        parameter.WithBody(
                            CodeLine.CreateVariable(closed, variable),
                            CodeLine.Assign(variable, temp));
                        parameter.WithBody(args);
                    });

                    return block;

                    //return Operation.InvokeReturn(Operation.NeestedLambda("ListCreator", typeof(object), parameter =>
                    //{
                    //    parameter.Returns(variable);
                    //    parameter.WithBody(
                    //        CodeLine.CreateVariable(closed, variable),
                    //        CodeLine.Assign(variable, temp));
                    //    parameter.WithBody(args);
                    //}), "Invoke");
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