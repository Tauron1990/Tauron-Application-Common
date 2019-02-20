using System.Collections.Generic;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace Tauron.Application.Ioc.BuildUp.Strategy
{
    [PublicAPI]
    public sealed class CompilationUnit
    {
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
        
        public string LifeTimeContext => VariableNamer.GetValiableName("LifeTimeContext");

        public string Input => VariableNamer.GetValiableName("InputObject");

        public string TargetName => VariableNamer.GetValiableName(nameof(TargetName));

        public VariableNamerImpl VariableNamer { get; private set; }

        public List<Expression> Expressions { get; } = new List<Expression>();

        public List<ParameterExpression> ParameterExpressions { get; } = new List<ParameterExpression>();

        public Dictionary<string, ParameterExpression> Variables { get; } = new Dictionary<string, ParameterExpression>();
    }
}