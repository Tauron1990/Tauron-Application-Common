using System;
using System.ComponentModel;
using JetBrains.Annotations;

namespace Tauron.Application.Models.Rules
{
    [PublicAPI]
    public class CompareRule : ModelRule
    {
        private PropertyDescriptor _propertyCache;

        public string MessageName { get; set; }

        public string OtherProperty { get; set; }

        public CompareRule()
            : base(nameof(CompareRule))
        {
        }

        public override ValidatorResult IsValidValue(object value, ValidatorContext context)
        {
            if(_propertyCache == null)
                _propertyCache = TypeDescriptor.GetProperties(context.Model).Find(OtherProperty, false);

            if (_propertyCache == null)
                return CreateResult(RuleMessages.CompareUnknownProperty, OtherProperty);

            object value2 = _propertyCache.GetValue(context.Model);

            if (Equals(value, value2)) return CreateResult();
            var name = DisplayNameHelper.GetDisplayName(context.ModelType.Name, OtherProperty, () => context.ModelType.GetProperty(OtherProperty));

            return CreateResult(RuleMessages.CompareMustMatch, context.Property.Metadata.DisplayName, name);
        }
    }
}
