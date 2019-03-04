using System;
using System.Reflection;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DefaultStrategy
{
    public class PropertyInjector : Injectorbase<PropertyInfo>
    {
        public PropertyInjector([NotNull] IMetadataFactory metadataFactory, [NotNull] PropertyInfo member, [NotNull] IResolverExtension[] resolverExtensions)
            : base(metadataFactory, member, resolverExtensions){}

        protected override Type MemberType => Member.PropertyType;

        protected override Action<object, object> GetInjectAction()
        {
            var member = Member;

            return (t, v) => member.SetValueFast(t, v);
        }

        protected override void Inject(object target, object value) => Member.SetValueFast(target, value);
    }
}