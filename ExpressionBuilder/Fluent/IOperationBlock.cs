using System.Collections.Generic;

namespace ExpressionBuilder.Fluent
{
    public interface IOperationBlock : IOperation, ICodeLine
    {
        IOperationBlock ReturnVar(string name);
        IOperationBlock WithBody(params ICodeLine[] lines);
        IOperationBlock WithBody(IEnumerable<ICodeLine> lines);
        void WithBody(ICodeLine firstCodeLine, params ICodeLine[] codeLines);
    }
}