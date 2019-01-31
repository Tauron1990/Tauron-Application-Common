using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Tauron.Application.Models.Rules
{
    public class RegexRule : ModelRule
    {
        private int _matchTimeoutInMilliseconds;
        private bool _matchTimeoutSet;
        
        public string Pattern { get; }

        public int MatchTimeoutInMilliseconds
        {
            get => _matchTimeoutInMilliseconds;
            set
            {
                _matchTimeoutInMilliseconds = value;
                _matchTimeoutSet = true;
            }
        }

        private Regex Regex { get; set; }



        public RegexRule(string pattern)
            : base(nameof(RegexRule)) =>
            Pattern = pattern;


        public override ValidatorResult IsValidValue(object value, ValidatorContext context)
        {
            if (Regex == null)
            {
                if (string.IsNullOrEmpty(Pattern))
                    return CreateResult(RuleMessages.RegularExpressionEmptyPattern);

                Regex = _matchTimeoutSet ? new Regex(Pattern, RegexOptions.None, TimeSpan.FromMilliseconds(MatchTimeoutInMilliseconds)) : new Regex(Pattern);
            }

            string text = Convert.ToString(value, CultureInfo.CurrentCulture);
            if (string.IsNullOrEmpty(text))
            {
                return CreateResult();
            }
            Match match = Regex.Match(text);

            return match.Success && match.Index == 0 && match.Length == text.Length ? CreateResult() : CreateResult(RuleMessages.RegexValidationError, context.DisplayName, Pattern);
        }
    }
}