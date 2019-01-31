namespace Tauron.Application.Models.Rules
{
    public class StringLengthRule : ModelRule
    {
        public int MaximumLength
        {
            get;
            private set;
        }
        
        public int MinimumLength { get; set; }

        public StringLengthRule(int maximumLenght)
            : base(nameof(StringLengthRule)) =>
            MaximumLength = maximumLenght;


        public override ValidatorResult IsValidValue(object value, ValidatorContext context)
        {
            if (MaximumLength < 0)
                return CreateResult(RuleMessages.StringLengthInvalidMaxLength);
            if (MaximumLength < MinimumLength)
                return CreateResult(RuleMessages.RangeMinGreaterThanMax, MaximumLength, MinimumLength);

            if (value == null) return CreateResult();
            int num = (value as string)?.Length ?? 0;

            return num >= MinimumLength && num <= MaximumLength ? CreateResult() : GetErrorMessage(context.DisplayName);

        }

        public ValidatorResult GetErrorMessage(string name)
        {
            string format = MinimumLength != 0 ? RuleMessages.StringLengthValidationErrorIncludingMinimum : RuleMessages.StringLengthValidationError;
            return CreateResult(format, name, MaximumLength, MinimumLength);
        }
    }
}