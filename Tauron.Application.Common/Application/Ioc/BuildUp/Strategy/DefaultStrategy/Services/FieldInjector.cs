using System;
using System.Reflection;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DefaultStrategy
{
    /// <summary>The field injector.</summary>
    public class FieldInjector : Injectorbase<FieldInfo>
    {
        public FieldInjector([NotNull] IMetadataFactory metadataFactory, [NotNull] FieldInfo member, [NotNull] [ItemNotNull] IResolverExtension[] resolverExtensions)
            : base(metadataFactory, member, resolverExtensions)
        {}

        protected override Type MemberType => Member.FieldType;

        protected override Action<object, object> GetInjectAction()
        {
            var member = Member;

            return (o, o2) => member.SetFieldFast(o, o2);
        }

        protected override void Inject(object target, object value) => Member.SetFieldFast(target, value);
    }
}