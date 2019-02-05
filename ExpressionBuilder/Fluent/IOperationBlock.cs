using System.Collections.Generic;

namespace ExpressionBuilder.Fluent
{
    public interface IOperationBlock : ILeftRightable
    {
        IOperationBlock ReturnVar(string name);
        IOperationBlock WithBody(params ICodeLine[] lines);
        IOperationBlock WithBody(IEnumerable<ICodeLine> lines);
        void WithBody(ICodeLine firstCodeLine, params ICodeLine[] codeLines);
    }
}