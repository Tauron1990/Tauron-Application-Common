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
using System.Runtime.CompilerServices;

namespace ExpressionBuilder.Parser
{
    public class ParseContext
    {
        private class ReferenceEqualsComp : IEqualityComparer<object>
        {
            bool IEqualityComparer<object>.Equals(object x, object y) => ReferenceEquals(x, y);

            public int GetHashCode(object obj) => RuntimeHelpers.GetHashCode(obj);
        }

        private readonly Dictionary<object, Expression> _consts = new Dictionary<object, Expression>(new ReferenceEqualsComp());
        private readonly List<ParseLevel> _parseLevels;

        internal LabelTarget ReturnLabel;
        internal string ReturnVariable;

        public ParseContext() => _parseLevels = new List<ParseLevel>();

        private int Level => Count - 1;

        private int Count => _parseLevels.Count;

        public ParseLevel Current => _parseLevels[Level];

        public string Pad => GetPad(Level + 1);

        public Expression PushConst(object @const, Type objt)
        {
            if(@const == null)
                return Expression.Constant(null, objt);

            if (_consts.TryGetValue(@const, out var exp))
            {
                if(exp.Type == objt)
                    return exp;
                return Expression.Constant(@const, objt);
            }

            exp = Expression.Constant(@const, objt);
            _consts[@const] = exp;
            return exp;
        }

        public void AddLevel()
        {
            var pl = new ParseLevel(this);
            _parseLevels.Add(pl);
        }

        public bool HasVariable(Variable var)
        {
            var i = Count - 1;
            while (i >= 0)
            {
                if (_parseLevels[i].HasVariable(var.Name)) return true;
                i--;
            }

            return false;
        }

        private string GetPad(int level)
        {
            var res = "";
            while (level >= 0)
            {
                res += " ";
                level--;
            }

            return res;
        }

        public Variable GetVariable(string name)
        {
            var i = Count - 1;
            while (i >= 0)
            {
                if (_parseLevels[i].HasVariable(name)) return _parseLevels[i].GetVariable(name);
                i--;
            }

            throw new Exception("Variable not found " + name);
        }

        public void RemoveLevel() => _parseLevels.RemoveAt(Level);
    }
}