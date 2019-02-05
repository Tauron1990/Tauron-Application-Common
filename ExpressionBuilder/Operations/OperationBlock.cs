using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ExpressionBuilder.CodeLines;
using ExpressionBuilder.Fluent;
using ExpressionBuilder.Parser;

namespace ExpressionBuilder.Operations
{
    public class OperationBlock : IOperationBlock
    {
        private readonly List<ICodeLine> _codeLines = new List<ICodeLine>();

        public OperationBlock(Type result) => ParsedType = result;

        public Type ParsedType { get; }

        public string ToString(ParseContext context)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(context.Pad);
            builder.AppendLine("{");
            context.AddLevel();

            foreach (var codeLine in _codeLines)
                builder.AppendLine(codeLine.ToString(context));

            context.RemoveLevel();
            builder.Append(context.Pad);
            builder.AppendLine("}");

            return builder.ToString();
        }

        public Expression ToExpression(ParseContext context)
        {
            context.AddLevel();
            List<Expression> exps = new List<Expression>();
            List<ParameterExpression> variables = new List<ParameterExpression>();

            foreach (var codeLine in _codeLines)
            {
                var expr = codeLine.ToExpression(context);
                
                if (codeLine is CreateVariable createVariable)
                {
                    variables.Add((ParameterExpression)expr);
                    expr = createVariable.DefaultInitialize(context);
                }

                exps.Add(expr);
            }

            var block = ParsedType == null ? Expression.Block(variables, exps.ToArray()) : Expression.Block(ParsedType, variables, exps.ToArray());

            context.RemoveLevel();

            return block;
        }

        public void PreParseExpression(ParseContext context)
        {
            foreach (var codeLine in _codeLines) codeLine.PreParseExpression(context);
        }

        public IOperationBlock WithBody(params ICodeLine[] lines)
        {
            _codeLines.AddRange(lines.Where(l => l != null));
            return this;
        }

        public IOperationBlock WithBody(IEnumerable<ICodeLine> lines)
        {
            _codeLines.AddRange(lines.Where(l => l != null));
            return this;
        }
    }
}