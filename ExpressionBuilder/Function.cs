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
using ExpressionBuilder.Utils;

namespace ExpressionBuilder
{
    public class Function : IBodyOrParameter, IFunctionReturn, IParsable
    {
        private readonly bool _asConstructor;
        private readonly List<ICodeLine> _codeLines;
        private readonly string _functionName;

        private readonly Dictionary<string, Variable> _inputParameters;

        private string _returnVariable;

        private Function(string functionName, bool asConstructor = false)
        {
            _inputParameters = new Dictionary<string, Variable>();
            _codeLines = new List<ICodeLine>();
            _functionName = functionName;
            _asConstructor = asConstructor;
        }

        public IBodyOrParameter WithParameter(Type type, string name)
        {
            if (_inputParameters.ContainsKey(name)) throw new ArgumentException($"Duplicate variable '{name}'", nameof(name));
            _inputParameters.Add(name, new Variable(type, name));
            return this;
        }

        public IFunctionReturn WithBody(ICodeLine firstCodeLine, params ICodeLine[] codeLines)
        {
            _codeLines.Add(firstCodeLine is If @if ? GetRootIf(@if) : firstCodeLine);
            foreach (var codeLine in codeLines.Where(cl => cl != null))
                _codeLines.Add(codeLine is If lastIf ? GetRootIf(lastIf) : codeLine);
            return this;
        }

        public IFunctionReturn WithBody(IEnumerable<ICodeLine> codeLines)
        {
            foreach (var codeLine in codeLines.Where(cl => cl != null))
                _codeLines.Add(codeLine is If lastIf ? GetRootIf(lastIf) : codeLine);
            return this;
        }

        public IBodyOrParameter WithParameter<TData>(string name) => WithParameter(typeof(TData), name);

        public LambdaExpression ToExpression()
        {
            var ctx = new ParseContext();
            ctx.AddLevel();
            return (LambdaExpression) ToExpression(ctx);
        }

        public IExpressionResult Returns(string variableName)
        {
            _returnVariable = variableName;
            return this;
        }

        public TData ToLambda<TData>() where TData : class
        {
            var resultExpression = ToExpression();
            var compiled = resultExpression.Compile();
            return compiled as TData;
        }

        public string ToString(ParseContext context)
        {
            context.ReturnVariable = _returnVariable;
            var functionBody = "(";
            var pl = context.Current;
            var first = true;
            foreach (var param in _inputParameters)
            {
                var pv = param.Value;
                pl.AddVariable(pv);
                if (!first) functionBody += ", ";
                first = false;
                functionBody += ReflectionUtil.TypeToString(pv.DataType);
                functionBody += " ";
                functionBody += pv.Name;
            }

            functionBody += ")";
            functionBody += "\n";
            functionBody += "{\n";
            foreach (var codeLine in _codeLines)
            {
                if (codeLine is CreateVariable createVariable) createVariable.DefaultInitialize(context);
                functionBody += context.Pad + codeLine.ToString(context) + ";\n";
            }

            var resultType = "void";
            if (!string.IsNullOrWhiteSpace(_returnVariable))
            {
                var resultVar = context.GetVariable(_returnVariable);
                resultType = ReflectionUtil.TypeToString(resultVar.DataType);
                functionBody += context.Pad + "return " + resultVar.Name + ";\n";
            }

            functionBody += "}";

            if (_asConstructor) resultType = "";
            else resultType = " " + resultType;

            functionBody = "public" + resultType + " " + _functionName + functionBody;

            return functionBody.Replace("\n", "\r\n");
        }

        public Expression ToExpression(ParseContext context)
        {
            PreParseExpression(context);

            var expressionsList = new List<Expression>();
            var exprParams = new List<ParameterExpression>();

            context.ReturnLabel = Expression.Label("return");
            Expression.Goto(context.ReturnLabel);

            var returnLabelExpression = Expression.Label(context.ReturnLabel);

            var pl = context.Current;

            foreach (var param in _inputParameters)
            {
                var pv = param.Value;
                var exp = Expression.Parameter(pv.DataType, param.Key);
                pv.Expression = exp;
                pl.AddVariable(pv);
                exprParams.Add(exp);
            }


            var listOfVars = new List<ParameterExpression>();
            foreach (var codeLine in _codeLines)
            {
                var expr = codeLine.ToExpression(context);

                //expressionsList.Add(expr);
                if (codeLine is CreateVariable createVariable)
                {
                    listOfVars.Add((ParameterExpression) expr);
                    expr = createVariable.DefaultInitialize(context);
                }

                expressionsList.Add(expr);
            }

            expressionsList.Add(returnLabelExpression);

            if (!string.IsNullOrWhiteSpace(_returnVariable))
            {
                var resultVar = context.GetVariable(_returnVariable);
                expressionsList.Add(resultVar.Expression);
            }

            var block = Expression.Block(
                listOfVars.ToArray(),
                expressionsList);

            return Expression.Lambda(block, exprParams);
        }

        public void PreParseExpression(ParseContext context)
        {
            var pl = context.Current;
            foreach (var param in _inputParameters)
            {
                var pv = param.Value;
                pl.AddVariable(pv);
            }

            foreach (var codeLine in _codeLines) codeLine.PreParseExpression(context);
            if (!string.IsNullOrWhiteSpace(_returnVariable))
                context.GetVariable(_returnVariable);
        }

        public Type ParsedType => null;

        public static IBodyOrParameter Create(string name = "Call", bool asConstructor = false) => new Function(name, asConstructor);

        private ICodeLine GetRootIf(If lastIf)
        {
            while (lastIf.ParentIf != null) lastIf = lastIf.ParentIf;
            return lastIf;
        }

        public override string ToString()
        {
            var ctx = new ParseContext();
            ctx.AddLevel();
            return ToString(ctx);
        }

        //private void PreParseExpression()
        //{
        //    var ctx = new ParseContext();
        //    ctx.AddLevel();
        //    PreParseExpression(ctx);
        //    ctx.RemoveLevel();
        //}
    }
}