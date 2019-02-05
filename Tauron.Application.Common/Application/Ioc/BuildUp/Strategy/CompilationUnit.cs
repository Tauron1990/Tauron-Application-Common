﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ExpressionBuilder;
using ExpressionBuilder.Fluent;
using JetBrains.Annotations;

namespace Tauron.Application.Ioc.BuildUp.Strategy
{
    [PublicAPI]
    public sealed class CompilationUnit //: IBodyOrParameter, IFunctionReturn
    {
        public class VariableNamerImpl
        {
            private int _currentVariable;
            private int _currentLevel = 1;

            public void AddLevel() => _currentLevel++;

            public void RemoveLevel() => _currentLevel--;

            public string GetValiableName(string name) => $"{name}_{_currentLevel}";

            public string GetRandomVariable()
            {
                _currentVariable++;
                return $"TempObject_{_currentVariable}_{_currentLevel}";
            }
        }

        private Action<ICodeLine[]> _addCode;

        private List<ICodeLine> _codeLines = new List<ICodeLine>();

        public Function RealFunction { get; }

        public string LifeTimeContext => VariableNamer.GetValiableName("LifeTimeContext");

        public string Input => VariableNamer.GetValiableName("InputObject");

        public string TargetName => VariableNamer.GetValiableName(nameof(TargetName));

        public VariableNamerImpl VariableNamer { get; } = new VariableNamerImpl();

        public CompilationUnit()
        {
            RealFunction = (Function) Function.Create("Create");
            RealFunction.Returns(TargetName);
            PushBody(RealFunction);
            _addCode(new [] { CodeLine.CreateVariable(typeof(object), TargetName) });
        }

        public override string ToString() => RealFunction.ToString();

        public bool AutoPush { get; private set; }

        private CompilationUnit InternalAddCode()
        {
            _addCode(_codeLines.ToArray());
            _codeLines.Clear();
            return this;
        }

        public CompilationUnit AddCode(params ICodeLine[] lines)
        {
            _codeLines.AddRange(lines);
            return this;
        }

        public CompilationUnit AddAndPush<TType>(TType line)
            where  TType : ICodeLine
        {
            bool autopush = AutoPush;
            _codeLines.Add(line);
            InternalAddCode();
            if(!autopush)
                PushBody(line);

            return this;
        }

        public void PushBody<TType>(TType body)
        {
            var type = typeof(TType);
            if (type == typeof(Function) || type == typeof(ICodeLine) || type == typeof(IOperation))
            {
                _addCode = codeLines => body.SafeCast<Function>().WithBody(codeLines);
                AutoPush = false;
            }
            else if (type == typeof(IIf))
            {
                _addCode = lines => PushBody(body.SafeCast<IIf>().Then(lines));
                AutoPush = true;
            }
            else if (type == typeof(IIfThen))
            {
                _addCode = lines => PushBody(body.SafeCast<IIfThen>().Else(lines));
                AutoPush = true;
            }
            else
                throw new ArgumentException(nameof(body));
        }

        public CompilationUnit WithBody(ICodeLine firstCodeLine, params ICodeLine[] codeLines)
        {
            RealFunction.WithBody(firstCodeLine, codeLines);
            return this;
        }

        public CompilationUnit WithParameter(Type type, string name)
        {
            RealFunction.WithParameter(type, name);
            return this;
        }

        public CompilationUnit WithParameter<TData>(string name) => WithParameter(typeof(TData), name);

        public LambdaExpression ToExpression()
        {
            if (_codeLines.Count == 0) return RealFunction.ToExpression();

            RealFunction.WithBody(_codeLines);
            _codeLines.Clear();
            return RealFunction.ToExpression();
        }

        public TData ToLambda<TData>() where TData : class => RealFunction.ToLambda<TData>();

        public CompilationUnit Returns(string variableName)
        {
            RealFunction.Returns(variableName);
            return this;
        }
    }
}