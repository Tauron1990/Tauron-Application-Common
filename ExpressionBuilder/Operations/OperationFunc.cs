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
using ExpressionBuilder.Fluent;
using ExpressionBuilder.Parser;
using ExpressionBuilder.Utils;

namespace ExpressionBuilder.Operations
{
    public class OperationFuncBase<TR> : IRightable
    {
        private readonly object _function;
        private readonly IOperation[] _parameters;
        protected Expression LambdaExpression;

        protected OperationFuncBase(object func, IOperation[] parameters)
        {
            _function = func;
            _parameters = parameters;
            ParsedType = typeof(TR);
        }

        public string ToString(ParseContext context)
        {
            //var functionTypes = Function.GetType().GenericTypeArguments;
            var dataType = ReflectionUtil.TypeToString(_function.GetType());
            var result = dataType + "(";
            for (var i = 0; i < _parameters.Length; i++)
            {
                if (i > 0) result += ", ";
                result += _parameters[i].ToString(context);
            }

            result += ")";

            return result;
        }

        public Expression ToExpression(ParseContext context)
        {
            var pars = new List<Expression>();
            foreach (var p in _parameters) pars.Add(p.ToExpression(context));

            return Expression.Invoke(LambdaExpression, pars);
        }

        public void PreParseExpression(ParseContext context)
        {
            foreach (var op in _parameters)
                op.PreParseExpression(context);
        }

        public Type ParsedType { get; }
    }

    public class OperationFunc<TR> : OperationFuncBase<TR>
    {
        public OperationFunc(Func<TR> func, IOperation[] parameters)
            : base(func, parameters)
        {
            if (parameters.Length != 0) throw new ArgumentException();
            Expression<Func<TR>> lambda = () => func();
            LambdaExpression = lambda;
        }
    }

    public class OperationFunc<TP1, TR> : OperationFuncBase<TR>
    {
        public OperationFunc(Func<TP1, TR> func, IOperation[] parameters)
            : base(func, parameters)
        {
            if (parameters.Length != 1) throw new ArgumentException();
            Expression<Func<TP1, TR>> lambda = p1 => func(p1);
            LambdaExpression = lambda;
        }
    }

    public class OperationFunc<TP1, TP2, TR> : OperationFuncBase<TR>
    {
        public OperationFunc(Func<TP1, TP2, TR> func, IOperation[] parameters)
            : base(func, parameters)
        {
            if (parameters.Length != 2) throw new ArgumentException();
            Expression<Func<TP1, TP2, TR>> lambda = (p1, p2) => func(p1, p2);
            LambdaExpression = lambda;
        }
    }
}