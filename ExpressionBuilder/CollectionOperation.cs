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
using ExpressionBuilder.Fluent;
using JetBrains.Annotations;

namespace ExpressionBuilder
{
    [PublicAPI]
    public static class CollectionOperation
    {
        public static IRightable CreateDictionary(Type key, Type value)
        {
            var dType = typeof(Dictionary<int, int>).MakeGenericType(key, value);
            return Operation.CreateInstance(dType);
        }

        public static IRightable CreateDictionary<TKey, TVal>() => CreateDictionary(typeof(TKey), typeof(TVal));

        public static IRightable CreateList(Type value)
        {
            var dType = typeof(List<int>).MakeGenericType(value);
            return Operation.CreateInstance(dType);
        }

        public static IRightable CreateList<TVal>() => CreateList(typeof(TVal));

        public static IRightable Count(IOperation variable) => Operation.Get(variable, "Count");

        public static IRightable Count(string variable) => Count(Operation.Variable(variable));

        public static IRightable Count(object value) => Count(Operation.Constant(value));
    }
}