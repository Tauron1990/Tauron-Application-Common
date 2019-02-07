using System.Collections.Generic;
using System.Linq;
using ExpressionBuilder.Fluent;
using JetBrains.Annotations;

namespace Tauron.Application.Ioc.BuildUp.Strategy
{
    [PublicAPI]
    public sealed class SubCompilitionUnit
    {
        private readonly CompilationUnit _unit;

        public SubCompilitionUnit(CompilationUnit unit) => _unit = unit;

        public string LifeTimeContext => _unit.LifeTimeContext;

        public string Input => _unit.Input;

        public string TargetName => _unit.TargetName;

        public CompilationUnit.VariableNamerImpl VariableNamer => _unit.VariableNamer;

        public void AddCode(ICodeLine firstCodeLine, params ICodeLine[] codeLines) => _unit.WithBody(firstCodeLine, codeLines);

        public void AddCode(params ICodeLine[] lines) => _unit.AddCode(lines);

        public void AddCode(IEnumerable<ICodeLine> lines) => _unit.AddCode(lines.ToArray());
    }
}