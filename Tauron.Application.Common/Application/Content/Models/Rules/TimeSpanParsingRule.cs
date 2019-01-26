using System;
using System.Globalization;
using JetBrains.Annotations;

namespace Tauron.Application.Models.Rules
{
    [PublicAPI]
    public sealed class TimeSpanParsingRule : ModelRule
    {
        private readonly bool _mustBePositive;

        public TimeSpanParsingRule(bool mustBePositive = true)
        : base(nameof(TimeSpanParsingRule))
        {
            _mustBePositive = mustBePositive;
        }

        public override ValidatorResult IsValidValue(object value, ValidatorContext context)
        {
            try
            {
                var span = TimeSpan.Parse((string) value, CultureInfo.CurrentUICulture);
                if (!_mustBePositive && span.Ticks >= 0) return CreateResult();

                //Get Message From Parse Exception
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                TimeSpan.Parse("-100000000000000000000000000000000");
                return CreateResult();
            }
            catch (Exception e) when (e is FormatException || e is OverflowException || e is ArgumentException)
            {
                return CreateResult(e.Message);
            }
        }
    }
}