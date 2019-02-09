using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ExpressionBuilder.Fluent;

namespace Tauron.Application.Ioc.BuildUp.Strategy
{
    public interface ICompilionTarget
    {
        void WithBody(params ICodeLine[] code);
        void WithBody(ICodeLine firstCodeLine, params ICodeLine[] codeLines);
        void WithParameter(Type type, string name);
        Expression ToExpression();
        void WithBody(IEnumerable<ICodeLine> codeLines);
        void Returns(string variableName);
        IOperation ToOperation();
        bool NoInput { get; }
    }
}