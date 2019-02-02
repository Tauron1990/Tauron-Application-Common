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
using ExpressionBuilder.Parser;
using ExpressionBuilder.Utils;

namespace ExpressionBuilder.Operations
{
    public class OperationInvokeReturn : IRightable
    {
        private readonly MethodInfo _staticMethod;
        private readonly string _methodName;
        private readonly IOperation[] _parameters;
        private readonly Type _staticDataType;
        private readonly IOperation _variable;

        private List<Type> _paramTypes;

        public OperationInvokeReturn(IOperation variable, string methodName, IOperation[] parameters)
        {
            _variable = variable;
            _methodName = methodName;
            _parameters = parameters;
        }

        public OperationInvokeReturn(Type dataType, string methodName, IOperation[] parameters)
        {
            _staticDataType = dataType;
            _methodName = methodName;
            _parameters = parameters;
        }

        public OperationInvokeReturn(MethodInfo staticMethod, IOperation[] parameters)
        {
            _staticMethod = staticMethod;
            _parameters = parameters;
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
            _paramTypes = new List<Type>();
            foreach (var op in _parameters)
            {
                op.PreParseExpression(context);
                _paramTypes.Add(op.ParsedType);
            }

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

            var methodInfo = _staticMethod ?? type.GetMethod(
                                 _methodName,
                                 BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static,
                                 null,
                                 _paramTypes.ToArray(),
                                 null);
            if (methodInfo != null)
                ParsedType = methodInfo.ReturnType;
        }

        public Type ParsedType { get; private set; }

        public Expression ToExpression(ParseContext context)
        {
            var pars = _parameters.Select(param => param.ToExpression(context)).ToList();

            if (_paramTypes == null)
            {
                _paramTypes = new List<Type>();
                foreach (var op in _parameters)
                {
                    op.PreParseExpression(context);
                    _paramTypes.Add(op.ParsedType);
                }
            }

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

            var method = ReflectionUtil.GetMethod(type, _methodName, _paramTypes);

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