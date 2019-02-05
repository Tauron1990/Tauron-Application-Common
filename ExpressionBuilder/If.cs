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
using System.Linq;
using System.Linq.Expressions;
using ExpressionBuilder.CodeLines;
using ExpressionBuilder.Fluent;
using ExpressionBuilder.Parser;

namespace ExpressionBuilder
{
    public class If : IIf, IIfThen
    {
        private readonly Condition _condition;
        private readonly List<ICodeLine> _elseCodeLines;
        private readonly List<ICodeLine> _thenCodeLines;
        internal readonly If ParentIf;

        internal If(Condition condition, If parentIf = null)
        {
            ParentIf = parentIf;
            _condition = condition ?? throw new ArgumentException();
            _thenCodeLines = new List<ICodeLine>();
            _elseCodeLines = new List<ICodeLine>();
        }

        public IIfThen Then(ICodeLine firstCodeLine, params ICodeLine[] codeLines)
        {
            if(firstCodeLine != null)
                _thenCodeLines.Add(firstCodeLine);
            foreach (var codeLine in codeLines.Where(cl => cl != null)) _thenCodeLines.Add(codeLine);
            return this;
        }

        public IIfThen Then(params ICodeLine[] codeLines)
        {
            foreach (var codeLine in codeLines.Where(cl => cl != null)) _thenCodeLines.Add(codeLine);
            return this;
        }

        public IIf ElseIf(Condition elseIfCondition)
        {
            var elseIf = new If(elseIfCondition, this);
            _elseCodeLines.Add(elseIf);
            return elseIf;
        }

        public ICodeLine Else(ICodeLine firstCodeLine, params ICodeLine[] codeLines)
        {
            if (firstCodeLine != null)
                _elseCodeLines.Add(firstCodeLine);
            foreach (var codeLine in codeLines.Where(cl => cl != null)) _elseCodeLines.Add(codeLine);
            return this;
        }

        public ICodeLine Else(params ICodeLine[] codeLines)
        {
            foreach (var codeLine in codeLines.Where(cl => cl != null)) _elseCodeLines.Add(codeLine);
            return this;
        }

        public string ToString(ParseContext context)
        {
            var result = "if(" + _condition.ToString(context) + ")\n";
            result += context.Pad + "{\n";
            context.AddLevel();

            foreach (var line in _thenCodeLines)
            {
                if (line is CreateVariable createVariable) createVariable.DefaultInitialize(context);
                result += context.Pad + line.ToString(context) + ";\n";
            }

            context.RemoveLevel();
            result += context.Pad + "}";
            if (_elseCodeLines.Count <= 0) return result;

            result += context.Pad + "\n";
            result += context.Pad + "else\n";
            result += context.Pad + "{\n";
            context.AddLevel();

            foreach (var line in _elseCodeLines)
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

            foreach (var line in _thenCodeLines) line.PreParseExpression(context);

            context.RemoveLevel();

            if (_elseCodeLines.Count <= 0) return;

            context.AddLevel();

            foreach (var line in _elseCodeLines) line.PreParseExpression(context);

            context.RemoveLevel();
        }

        public Type ParsedType => null;

        public Expression ToExpression(ParseContext context)
        {
            var conditionExpression = _condition.ToExpression(context);
            context.AddLevel();

            var thenLine = new List<Expression>();
            var listOfThenVars = new List<ParameterExpression>();
            foreach (var line in _thenCodeLines)
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

            var elseLine = new List<Expression>();
            if (_elseCodeLines.Count <= 0) return Expression.IfThen(conditionExpression, thenBlock);

            context.AddLevel();
            var listOfElseVars = new List<ParameterExpression>();
            foreach (var line in _elseCodeLines)
            {
                var expLine = line.ToExpression(context);

                if (line is CreateVariable createVariable)
                {
                    listOfElseVars.Add((ParameterExpression) expLine);
                    expLine = createVariable.DefaultInitialize(context);
                }

                elseLine.Add(expLine);
            }

            context.RemoveLevel();
            var elseBlock = Expression.Block(listOfElseVars, elseLine);
            return Expression.IfThenElse(conditionExpression, thenBlock, elseBlock);
        }
    }
}