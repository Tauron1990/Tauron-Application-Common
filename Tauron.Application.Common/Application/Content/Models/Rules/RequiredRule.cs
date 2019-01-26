using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Tauron.Application.Models.Rules
{
    [PublicAPI]
    public sealed class RequiredRule : ModelRule
    {
        public RequiredRule()
        : base(nameof(RequiredRule)) { }

        public string FieldName { get; set; }

        public bool AllowStringEmpty { get; set; }

        public override ValidatorResult IsValidValue(object value, ValidatorContext context)
        {
            if (value == null) return Error(context);

            if (!(value is string str) || AllowStringEmpty) return CreateResult();

            return string.IsNullOrWhiteSpace(str) ? Error(context) : CreateResult();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ValidatorResult Error(ValidatorContext context) => CreateResult(RuleMessages.RequireRuleError.SFormat(context.Property.Name));
    }
}