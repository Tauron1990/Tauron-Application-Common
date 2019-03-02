using System;
using System.Reflection;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    public class PropertyInjector : Injectorbase<PropertyInfo>
    {
        public PropertyInjector([NotNull] IMetadataFactory metadataFactory, [NotNull] PropertyInfo member, [NotNull] IResolverExtension[] resolverExtensions)
            : base(metadataFactory, member, resolverExtensions){}

        protected override Type MemberType => Member.PropertyType;
        protected override void Inject(object target, object value) => Member.SetValueFast(target, value);
    }
}