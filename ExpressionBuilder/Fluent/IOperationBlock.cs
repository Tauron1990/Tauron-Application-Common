using System.Collections.Generic;

namespace ExpressionBuilder.Fluent
{
    public interface IOperationBlock : ICodeLine
    {
        IOperationBlock WithBody(params ICodeLine[] lines);
        IOperationBlock WithBody(IEnumerable<ICodeLine> lines);
    }
}