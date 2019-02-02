using System;
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
        public class DefaultVariableNames
        {
            public const string LifeTimeContext = "LifeTimeContext";

            public const string Input = "InputObject";
        }

        private Action<ICodeLine, ICodeLine[]> _addCode;

        private List<ICodeLine> _codeLines = new List<ICodeLine>();

        public Function RealFunction { get; }

        public Dictionary<string, Type> Variabeles { get; set; }

        public static string TargetName => nameof(TargetName);

        public CompilationUnit()
        {
            RealFunction = (Function) Function.Create("Create");
            WithParameter(typeof(object), TargetName);
            RealFunction.Returns(TargetName);
            PushBody(RealFunction);
        }

        public override string ToString() => RealFunction.ToString();

        public bool AutoPush { get; private set; }

        private CompilationUnit InternalAddCode(ICodeLine firstCodeLine)
        {
            _addCode(firstCodeLine, _codeLines.ToArray());
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
            InternalAddCode(line);
            if(!autopush)
                PushBody(line);

            return this;
        }

        public void PushBody<TType>(TType body)
        {
            var type = typeof(TType);
            if (type == typeof(Function) || type == typeof(ICodeLine) || type == typeof(IOperation))
            {
                _addCode = (codeLine, codeLines) => body.SafeCast<Function>().WithBody(codeLine, codeLines);
                AutoPush = false;
            }

            if (type == typeof(IIf))
            {
                _addCode = (line, lines) => PushBody(body.SafeCast<IIf>().Then(line, lines));
                AutoPush = true;
            }

            if (type == typeof(IIfThen))
            {
                _addCode = (line, lines) => PushBody(body.SafeCast<IIfThen>().Else(line, lines));
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
            Variabeles[name] = type;
            return this;
        }

        public CompilationUnit WithParameter<TData>(string name) => WithParameter(typeof(TData), name);

        public LambdaExpression ToExpression() => RealFunction.ToExpression();

        public TData ToLambda<TData>() where TData : class => RealFunction.ToLambda<TData>();

        public CompilationUnit Returns(string variableName)
        {
            RealFunction.Returns(variableName);
            return this;
        }
    }
}