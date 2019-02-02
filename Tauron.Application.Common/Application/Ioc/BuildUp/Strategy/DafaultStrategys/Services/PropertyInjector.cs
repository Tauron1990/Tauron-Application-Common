using System;
using System.Reflection;
using ExpressionBuilder;
using ExpressionBuilder.Fluent;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    public class PropertyInjector : Injectorbase<PropertyInfo>
    {
        public PropertyInjector([NotNull] IMetadataFactory metadataFactory, [NotNull] PropertyInfo member, [NotNull] IResolverExtension[] resolverExtensions)
            : base(metadataFactory, member, resolverExtensions){}

        protected override Type MemberType => Member.PropertyType;

        protected override void Inject(CompilationUnit target, IRightable value) => target.AddCode(Operation.Set(CompilationUnit.TargetName, Member, value));
    }
}