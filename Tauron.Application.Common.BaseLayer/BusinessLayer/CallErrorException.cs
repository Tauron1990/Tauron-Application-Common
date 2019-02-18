using System;
using System.Collections.Generic;

namespace Tauron.Application.Common.BaseLayer.BusinessLayer
{
    public class CallErrorException : Exception
    {
        public CallErrorException(ErrorReturn error)
        {
            Errors = error.Errors;
            Rule = error.RuleBase;
        }

        public IEnumerable<object> Errors { get; }

        public IRuleBase Rule { get; }
    }
}