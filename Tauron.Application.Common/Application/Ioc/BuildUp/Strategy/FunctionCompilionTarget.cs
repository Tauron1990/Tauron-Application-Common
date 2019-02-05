using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ExpressionBuilder;
using ExpressionBuilder.Fluent;

namespace Tauron.Application.Ioc.BuildUp.Strategy
{
    public class FunctionCompilionTarget : ICompilionTarget
    {
        private readonly Function _function;

        public FunctionCompilionTarget(string returnVariable)
        {
            _function = (Function)Function.Create("Create");
            _function.Returns(returnVariable);
        }

        public void WithBody(params ICodeLine[] code) => _function.WithBody(code);

        public void WithBody(ICodeLine firstCodeLine, params ICodeLine[] codeLines) => _function.WithBody(firstCodeLine, codeLines);

        public void WithParameter(Type type, string name) => _function.WithParameter(type, name);

        public Expression ToExpression() => _function.ToExpression();

        public void WithBody(IEnumerable<ICodeLine> codeLines) => _function.WithBody(codeLines);

        public void Returns(string variableName) => _function.Returns(variableName);

        public ILeftRightable ToOperation() => throw new NotSupportedException("Function Can not Convert to Operation!");
    }
}