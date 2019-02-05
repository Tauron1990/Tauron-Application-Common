using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ExpressionBuilder;
using ExpressionBuilder.Fluent;
using ExpressionBuilder.Parser;

namespace Tauron.Application.Ioc.BuildUp.Strategy
{
    public class BlockFunctionTarget : ICompilionTarget
    {
        private readonly IOperationBlock _operationBlock;
        private string _returnVariable;

        public BlockFunctionTarget(string returnVariable)
        {
            _operationBlock = Operation.Block(typeof(object), null);
            _returnVariable = returnVariable;
            _operationBlock.WithBody(CodeLine.CreateVariable<object>(returnVariable));
        }

        public void WithBody(params ICodeLine[] code) => _operationBlock.WithBody(code);

        public void WithBody(ICodeLine firstCodeLine, params ICodeLine[] codeLines) => _operationBlock.WithBody(firstCodeLine, codeLines);

        public void WithParameter(Type type, string name) { }

        public Expression ToExpression()
        {
            _operationBlock.ReturnVar(_returnVariable);
            _operationBlock.PreParseExpression(new ParseContext());
            return _operationBlock.ToExpression(new ParseContext());
        }

        public void WithBody(IEnumerable<ICodeLine> codeLines) => _operationBlock.WithBody(codeLines);

        public void Returns(string variableName) => _returnVariable = variableName;

        public ILeftRightable ToOperation()
        {
            _operationBlock.ReturnVar(_returnVariable);
            return _operationBlock;
        }
    }
}