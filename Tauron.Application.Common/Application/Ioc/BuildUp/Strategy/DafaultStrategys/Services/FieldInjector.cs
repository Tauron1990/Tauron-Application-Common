using System;
using System.Linq.Expressions;
using System.Reflection;
using ExpressionBuilder;
using ExpressionBuilder.Fluent;
using ExpressionBuilder.Parser;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    /// <summary>The field injector.</summary>
    public class FieldInjector : Injectorbase<FieldInfo>
    {
        private class FieldSetter : ICodeLine
        {
            private readonly FieldInfo _info;
            private readonly IRightable _target;
            private readonly IRightable _value;

            public FieldSetter(FieldInfo info, IRightable target, IRightable value)
            {
                _info = info;
                _target = target;
                _value = value;
            }

            public Type ParsedType => _info.FieldType;

            public string ToString(ParseContext context) => $"{_info.Name} = {_value.ToString(context)}";

            public Expression ToExpression(ParseContext context)
            {
                MemberExpression fieldExp = Expression.Field(_target.ToExpression(context), _info);
                return Expression.Assign(fieldExp, _value.ToExpression(context));
            }

            public void PreParseExpression(ParseContext context)
            {
                _target.PreParseExpression(context);
                _value.PreParseExpression(context);
            }
        }

        public FieldInjector([NotNull] IMetadataFactory metadataFactory, [NotNull] FieldInfo member, [NotNull] [ItemNotNull] IResolverExtension[] resolverExtensions)
            : base(metadataFactory, member, resolverExtensions)
        {}

        protected override Type MemberType => Member.FieldType;

        protected override void Inject(CompilationUnit target, IRightable value)
        {
            target.AddCode(new FieldSetter(Member, Operation.Variable(CompilationUnit.TargetName), value));
            Member.SetValue(target, value);
        }
    }
}