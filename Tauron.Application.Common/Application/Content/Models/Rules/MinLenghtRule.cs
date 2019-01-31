using System;
using System.Collections;

namespace Tauron.Application.Models.Rules
{
    public class MinLenghtRule : ModelRule
    {
        public MinLenghtRule(int minLength)
            : base(nameof(MinLenghtRule)) =>
            Length = minLength;

        public int Length { get; }

        public override ValidatorResult IsValidValue(object value, ValidatorContext context)
        {
            if (Length < 0)
                return CreateResult(RuleMessages.InvalidMinLength);
            if (value == null)
                return CreateResult();

            int num;

            switch (value)
            {
                case string text:
                    num = text.Length;
                    break;
                case Array array:
                    num = array.Length;
                    break;
                case IEnumerable enumerable:
                    num = enumerable.Count();
                    break;
                default:
                    num = 0;
                    break;
            }

            return num >= Length ? CreateResult() : CreateResult(RuleMessages.MinLengthValidationError, context.DisplayName, Length);
        }
    }
}