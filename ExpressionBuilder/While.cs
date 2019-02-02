// ===========================================================
// Copyright (c) 2014-2015, Enrico Da Ros/kendar.org
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
// 
// * Redistributions of source code must retain the above copyright notice, this
//   list of conditions and the following disclaimer.
// 
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// ===========================================================


using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ExpressionBuilder.CodeLines;
using ExpressionBuilder.Fluent;
using ExpressionBuilder.Parser;

namespace ExpressionBuilder
{
    public class While : IWhile, ICodeLine
    {
        private readonly List<ICodeLine> _codeLines;
        private Condition _condition;

        internal While(Condition condition)
        {
            _condition = condition ?? throw new ArgumentException();
            _codeLines = new List<ICodeLine>();
        }


        public string ToString(ParseContext context)
        {
            var result = "while(" + _condition.ToString(context) + ")\n";
            result += context.Pad + "{\n";
            context.AddLevel();

            foreach (var line in _codeLines)
            {
                if (line is CreateVariable createVariable) createVariable.DefaultInitialize(context);
                result += context.Pad + line.ToString(context) + ";\n";
            }

            context.RemoveLevel();
            result += context.Pad + "}";
            return result;
        }

        public void PreParseExpression(ParseContext context)
        {
            //var pl = context.Current;
            _condition.PreParseExpression(context);
            context.AddLevel();

            foreach (var line in _codeLines) line.PreParseExpression(context);

            context.RemoveLevel();
        }

        public Type ParsedType => null;

        public Expression ToExpression(ParseContext context)
        {
            var conditionExpression = _condition.ToExpression(context);
            context.AddLevel();

            var thenLine = new List<Expression>();
            var listOfThenVars = new List<ParameterExpression>();
            foreach (var line in _codeLines)
            {
                var expLine = line.ToExpression(context);

                if (line is CreateVariable createVariable)
                {
                    listOfThenVars.Add((ParameterExpression) expLine);
                    expLine = createVariable.DefaultInitialize(context);
                }

                thenLine.Add(expLine);
            }

            var thenBlock = Expression.Block(listOfThenVars.ToArray(), thenLine);

            context.RemoveLevel();

            var label = Expression.Label(Guid.NewGuid().ToString());
            var ifThenElse = Expression.IfThenElse(
                conditionExpression,
                thenBlock,
                Expression.Break(label));
            return Expression.Loop(ifThenElse, label);
        }

        public ICodeLine Do(ICodeLine firstCodeLine, params ICodeLine[] codeLines)
        {
            _codeLines.Add(firstCodeLine);
            foreach (var codeLine in codeLines) _codeLines.Add(codeLine);
            return this;
        }
    }
}