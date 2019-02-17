using System;
using System.Linq;

namespace Tauron.Application.Models.Rules
{
    public sealed class DefaultValidator : IValidator
    {
        public static readonly DefaultValidator DefaultValidatorInstance = new DefaultValidator();

        public PropertyIssue[] Validate(ValidatorContext context, object value)
        {
            var rules = context.Property.Metadata.GetModelRules();
            if (rules == null) return Array.Empty<PropertyIssue>();

            return rules.Select(r => r.IsValidValue(value, context))
                    .Where(e => !e.Succseeded)
                    .Select(r => new PropertyIssue(context.Property.Name, value, r.Message ?? value?.ToString() ?? string.Empty))
                    .ToArray();
        }
    }
}