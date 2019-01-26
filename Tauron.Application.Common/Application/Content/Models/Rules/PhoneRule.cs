using System;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace Tauron.Application.Models.Rules
{
    [PublicAPI]
    public class PhoneRule : ModelRule
    {
        public PhoneRule()
            : base(nameof(PhoneRule))
        {
            
        }

        private static Regex _regex = CreateRegEx();

        public override ValidatorResult IsValidValue(object value, ValidatorContext context)
        {
            switch (value)
            {
                case null:
                case string text when _regex.Match(text).Length > 0:
                    return CreateResult();
                default:
                    return CreateResult(RuleMessages.PhoneInvalid);
            }
        }


        private static Regex CreateRegEx()
        {
            TimeSpan matchTimeout = TimeSpan.FromSeconds(2.0);
            try
            {
                if (AppDomain.CurrentDomain.GetData("REGEX_DEFAULT_MATCH_TIMEOUT") == null)
                {
                    return new Regex("^(\\+\\s?)?((?<!\\+.*)\\(\\+?\\d+([\\s\\-\\.]?\\d+)?\\)|\\d+)([\\s\\-\\.]?(\\(\\d+([\\s\\-\\.]?\\d+)?\\)|\\d+))*(\\s?(x|ext\\.?)\\s?\\d+)?$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled, matchTimeout);
                }
            }
            catch
            {
            }
            return new Regex("^(\\+\\s?)?((?<!\\+.*)\\(\\+?\\d+([\\s\\-\\.]?\\d+)?\\)|\\d+)([\\s\\-\\.]?(\\(\\d+([\\s\\-\\.]?\\d+)?\\)|\\d+))*(\\s?(x|ext\\.?)\\s?\\d+)?$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled);
        }
    }
}