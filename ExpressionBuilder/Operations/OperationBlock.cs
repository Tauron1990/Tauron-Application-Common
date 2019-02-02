using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ExpressionBuilder.Fluent;
using ExpressionBuilder.Parser;

namespace ExpressionBuilder.Operations
{
    public class OperationBlock : IRightable, IBodyOrParameter, IFunctionReturn, ICodeLine
    {
        public Function Function { get; }

        public OperationBlock(string name, Type type)
        {
            ParsedType = type;
            Function = (Function) Function.Create(name);
        }

        public IFunctionReturn WithBody(ICodeLine firstCodeLine, params ICodeLine[] codeLines)
        {
            Function.WithBody(firstCodeLine, codeLines);
            return this;
        }

        public IFunctionReturn WithBody(IEnumerable<ICodeLine> codeLines)
        {
            Function.WithBody(codeLines);
            return this;
        }

        public IBodyOrParameter WithParameter(Type type, string name)
        {
            Function.WithParameter(type, name);
            return this;
        }

        public IBodyOrParameter WithParameter<TData>(string name)
        {
            Function.WithParameter<TData>(name);
            return this;
        }

        public LambdaExpression ToExpression() => throw new NotSupportedException();

        public TData ToLambda<TData>() where TData : class => throw new NotSupportedException();

        public IExpressionResult Returns(string variableName)
        {
            Function.Returns(variableName);
            return this;
        }

        public Type ParsedType { get; }

        public string ToString(ParseContext context) => Function.ToString(context);

        public Expression ToExpression(ParseContext context) => Function.ToExpression(context);

        public void PreParseExpression(ParseContext context)
        {
        }
    }
}