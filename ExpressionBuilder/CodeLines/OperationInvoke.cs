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
using System.Reflection;
using ExpressionBuilder.Fluent;
using ExpressionBuilder.Operations;
using ExpressionBuilder.Parser;
using ExpressionBuilder.Utils;

namespace ExpressionBuilder.CodeLines
{
    public class OperationInvoke : IOperation, ICodeLine
    {
        private readonly string _methodName;
        private readonly IOperation[] _parameters;
        private readonly Type _staticDataType;
        private readonly IOperation _variable;
        private readonly MethodInfo _methodInfo;
        private MethodCallDescriptor _methodCallDescriptor;

        private List<Type> _paramTypes;

        public OperationInvoke(IOperation variable, string methodName, IOperation[] parameters)
        {
            _variable = variable;
            _methodName = methodName;
            _parameters = parameters;
        }

        public OperationInvoke(Type dataType, string methodName, IOperation[] parameters)
        {
            _methodName = methodName;
            _parameters = parameters;
            _staticDataType = dataType;
        }

        public OperationInvoke(IOperation variable, MethodInfo methodName, IOperation[] parameters)
        {
            _methodInfo = methodName;
            _parameters = parameters;
            _variable = variable;
        }

        public string ToString(ParseContext context)
        {
            var result = _staticDataType == null ? _variable.ToString(context) : ReflectionUtil.TypeToString(_staticDataType);
            result += "." + _methodName + "(";
            for (var i = 0; i < _parameters.Length; i++)
            {
                if (i > 0) result += ", ";
                result += _parameters[i].ToString(context);
            }

            return result + ")";
        }

        public void PreParseExpression(ParseContext context)
        {
            _variable.PreParseExpression(context);

            _paramTypes = new List<Type>();
            foreach (var op in _parameters)
            {
                op.PreParseExpression(context);
                _paramTypes.Add(op.ParsedType);
            }

            ParsedType = null;

            if(_methodInfo == null) return;

            _methodCallDescriptor = ReflectionUtil.EvaluateCorrectness(_methodInfo.GetParameters(), _paramTypes);
            _methodCallDescriptor.Method = _methodInfo;
        }

        public Type ParsedType { get; private set; }

        public Expression ToExpression(ParseContext context)
        {
            var pars = _parameters.Select(param => param.ToExpression(context)).ToList();

            var type = _staticDataType;
            if (_staticDataType == null)
            {
                type = _variable.ParsedType;
                if (_variable is OperationVariable operationVariable)
                {
                    var variable = context.GetVariable(operationVariable.Name);
                    type = variable.DataType;
                }
            }

            var method = _methodCallDescriptor ?? ReflectionUtil.GetMethod(type, _methodName, _paramTypes);

            if (method.GoodFrom >= 0)
            {
                var startDefault = method.GoodFrom;
                while (startDefault < method.ParamValues.Count)
                {
                    pars.Add(Operation.Constant(method.ParamValues[method.GoodFrom]).ToExpression(context));
                    startDefault++;
                }
            }

            var my = (MethodInfo) method.Method;
            return (my.Attributes & MethodAttributes.Static) == 0
                ? Expression.Call(_variable.ToExpression(context), (MethodInfo) method.Method, pars)
                : Expression.Call((MethodInfo) method.Method, pars);
        }
    }
}