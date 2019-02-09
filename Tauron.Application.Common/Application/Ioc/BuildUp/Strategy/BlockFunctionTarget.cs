using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ExpressionBuilder;
using ExpressionBuilder.Fluent;

namespace Tauron.Application.Ioc.BuildUp.Strategy
{
    public class BlockFunctionTarget : ICompilionTarget
    {
        private readonly string _returnVariable;
        private readonly SubCompilitionUnit _unit;

        public BlockFunctionTarget(string returnVariable, SubCompilitionUnit unit)
        {
            _returnVariable = returnVariable;
            _unit = unit;
            unit.AddCode(CodeLine.CreateVariable<object>(returnVariable));
        }

        public void WithBody(params ICodeLine[] code) => _unit.AddCode(code);

        public void WithBody(ICodeLine firstCodeLine, params ICodeLine[] codeLines) => _unit.AddCode(firstCodeLine, codeLines);

        public void WithParameter(Type type, string name) { }

        public Expression ToExpression() => throw new NotSupportedException();

        public void WithBody(IEnumerable<ICodeLine> codeLines) => _unit.AddCode(codeLines);

        public void Returns(string variableName) => throw new NotSupportedException();

        public IOperation ToOperation() => Operation.Variable(_returnVariable);

        public bool NoInput => true;
    }
}