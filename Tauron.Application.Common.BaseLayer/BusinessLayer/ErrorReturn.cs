using System.Collections.Generic;

namespace Tauron.Application.Common.BaseLayer.BusinessLayer
{
    public class ErrorReturn : Return
    {
        public IEnumerable<object> Errors { get; }

        public IRuleBase RuleBase { get; }

        public override bool Error { get; } = true;

        public ErrorReturn(IRuleBase rule)
        {
            Errors = rule.Errors;
            RuleBase = rule;
        }
    }
}