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
using System.Reflection;
using ExpressionBuilder.CodeLines;
using ExpressionBuilder.Fluent;
using ExpressionBuilder.Operations;
using JetBrains.Annotations;

namespace ExpressionBuilder
{
    [PublicAPI]
    public static class Operation
    {
        public static ILeftRightable Variable(string name) => new OperationVariable(name);

        public static IRightable Constant(object value)
        {
            if (value is OperationConst @const) return @const;
            return new OperationConst(value);
        }

        public static OperationBlock NeestedLambda(string name, Type type, Action<IBodyOrParameter> block)
        {
            var op = new OperationBlock(name, type);
            block(op);
            return op;
        }

        public static IRightable InvokeReturn(MethodInfo staticMethod, params IOperation[] parameters) => new OperationInvokeReturn(staticMethod, parameters);

        public static IRightable InvokeReturn(IOperation variable, string methodName, params IOperation[] parameters) => new OperationInvokeReturn(variable, methodName, parameters);

        public static IRightable InvokeReturn(string variable, string methodName, params IOperation[] parameters) => InvokeReturn(Variable(variable), methodName, parameters);

        public static IRightable InvokeReturn(Type dataType, string methodName, params IOperation[] parameters) => new OperationInvokeReturn(dataType, methodName, parameters);

        public static IRightable InvokeReturn<TData>(string methodName, params IOperation[] parameters) => InvokeReturn(typeof(TData), methodName, parameters);

        public static ICodeLine Invoke(IOperation variable, string methodName, params IOperation[] parameters) => new OperationInvoke(variable, methodName, parameters);

        public static ICodeLine Invoke(string variable, string methodName, params IOperation[] parameters) => Invoke(Variable(variable), methodName, parameters);

        public static ICodeLine Invoke(IOperation variable, MethodInfo methodName, params IOperation[] parameters) => new OperationInvoke(variable, methodName, parameters);

        public static ICodeLine Invoke(string variable, MethodInfo methodName, params IOperation[] parameters) => Invoke(Variable(variable), methodName, parameters);

        public static ICodeLine Invoke(Type dataType, string methodName, params IOperation[] parameters) => new OperationInvoke(dataType, methodName, parameters);

        public static ICodeLine Invoke<TData>(string methodName, params IOperation[] parameters) => Invoke(typeof(TData), methodName, parameters);

        public static IRightable Get(IOperation variable, string propertyName) => InvokeReturn(variable, "get_" + propertyName);

        public static IRightable Get(string variable, string propertyName) => Get(Variable(variable), propertyName);

        public static ICodeLine Set(IOperation variable, string propertyName, IOperation value) => Invoke(variable, "set_" + propertyName, value);

        public static ICodeLine Set(string variable, string propertyName, IOperation value) => Set(Variable(variable), propertyName, value);

        public static ICodeLine Set(IOperation variable, PropertyInfo propertyInfo, IOperation value) => Invoke(variable, propertyInfo.SetMethod, value);

        public static ICodeLine Set(string variable, PropertyInfo propertyInfo, IOperation value) => Set(Variable(variable), propertyInfo, value);

        public static IRightable Null() => Constant(null);

        public static IRightable Cast(IOperation variable, Type toType) => new OperationCast(variable, toType);

        public static IRightable Cast<TData>(IOperation variable) => Cast(variable, typeof(TData));

        public static IRightable Cast(string variable, Type toType) => Cast(Variable(variable), toType);

        public static IRightable Cast<TData>(string variable) => Cast(Variable(variable), typeof(TData));

        public static IRightable CastConst(object value, Type toType) => Cast(Constant(value), toType);


        public static IRightable CastConst<TData>(object value) => Cast(Constant(value), typeof(TData));

        public static IRightable CreateArray(Type dataType, params IRightable[] variables) => new OperationNewArrayInit(dataType, variables);

        public static IRightable CreateInstance(Type dataType, params IRightable[] variables) => new OperationNew(dataType, variables);

        public static IRightable CreateInstance<TData>(params IRightable[] variables) => new OperationNew(typeof(TData), variables);

        public static IRightable CreateInstance(Type dataType, IEnumerable<Type> types, params IRightable[] variables)
        {
            var generic = dataType.MakeGenericType(types.ToArray());
            return CreateInstance(generic, variables);
        }

        public static IRightable CreateInstance<TData>(IEnumerable<Type> types, params IRightable[] variables)
        {
            var generic = typeof(TData).MakeGenericType(types.ToArray());
            return CreateInstance(generic, variables);
        }

        public static IRightable CreateInstance(ConstructorInfo constructorInfo, params IRightable[] parameters) => new OperationNew(constructorInfo, parameters);


        public static IRightable Func<TR>(Func<TR> func, params IOperation[] parameters) => new OperationFunc<TR>(func, parameters);

        public static IRightable Func<TP1, TR>(Func<TP1, TR> func, params IOperation[] parameters) => new OperationFunc<TP1, TR>(func, parameters);

        public static IRightable Func<TP1, TP2, TR>(Func<TP1, TP2, TR> func, params IOperation[] parameters) => new OperationFunc<TP1, TP2, TR>(func, parameters);

        public static ICodeLine Action<TP1>(Action<TP1> action, params IOperation[] parameters) => new OperationAction<TP1>(action, parameters);

        public static ICodeLine Action<TP1, TP2>(Action<TP1, TP2> action, params IOperation[] parameters) => new OperationAction<TP1, TP2>(action, parameters);
    }
}