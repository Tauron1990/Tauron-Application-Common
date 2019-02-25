using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace Tauron.Application.Ioc.BuildUp.Strategy
{
    [PublicAPI]
    public sealed class CompilationUnit
    {
        private class ConstCacheKey : IEquatable<ConstCacheKey>
        {
            public object Element { get; }

            public Type Type { get; }

            public ConstCacheKey(object element, Type type)
            {
                Element = element;
                Type = type;
            }

            public bool Equals(ConstCacheKey other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(Element, other.Element) && Equals(Type, other.Type);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != GetType()) return false;
                return Equals((ConstCacheKey) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Element != null ? Element.GetHashCode() : 0) * 397) ^ (Type != null ? Type.GetHashCode() : 0);
                }
            }
        }

        private Dictionary<ConstCacheKey, Expression> _constDictionary = new Dictionary<ConstCacheKey, Expression>();

        public class VariableNamerImpl
        {
            private int _currentVariable;
            private int _nextLevel = 1;
            private int _actualLevel = 1;

            public void AddLevel()
            {
                _nextLevel++;
                _actualLevel = _nextLevel;
            }

            public void RemoveLevel() => _actualLevel = 1;

            public string GetValiableName(string name) => $"{name}_{_actualLevel}";

            public string GetRandomVariable()
            {
                _currentVariable++;
                return $"TempObject_{_currentVariable}_{_actualLevel}";
            }
        }
        
        public string LifeTimeContext => "LifeTimeContext";

        public string Input => "InputObject";

        public string TargetName => nameof(TargetName);

        public VariableNamerImpl VariableNamer { get; private set; }

        public List<Expression> Expressions { get; } = new List<Expression>();

        public List<ParameterExpression> ParameterExpressions { get; } = new List<ParameterExpression>();

        public Dictionary<string, ParameterExpression> Variables { get; } = new Dictionary<string, ParameterExpression>();

        public Expression GetConst(object obj, Type type)
        {
            if (obj == null && type == null)
                type = typeof(object);
            if (type == null)
                type = obj.GetType();

            var key = new ConstCacheKey(obj, type);

            if (_constDictionary.TryGetValue(key, out var exp)) return exp;

            exp = Expression.Constant(obj, type);
            _constDictionary[key] = exp;

            return exp;
        }

        public Expression GetConst(object obj) => GetConst(obj, obj.GetType());

        public ParameterExpression GetRandomVariable(Type type)
        {
            string name = VariableNamer.GetRandomVariable();
            var exp = Expression.Variable(type, name);

            Variables[name] = exp;

            return exp;
        }

        public ParameterExpression GetOrAddVariable(Type type, string name)
        {
            name = VariableNamer.GetValiableName(name);
            if (Variables.TryGetValue(name, out var exp)) return exp;

            exp = Expression.Variable(type, name);
            Variables[name] = exp;

            return exp;
        }
    }
}