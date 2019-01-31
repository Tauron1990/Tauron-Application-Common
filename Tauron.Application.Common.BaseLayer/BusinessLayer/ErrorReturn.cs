using System.Collections.Generic;

namespace Tauron.Application.Common.BaseLayer.BusinessLayer
{
    public class ErrorReturn : Return
    {
        public IEnumerable<object> Errors { get; }

        public override bool Error { get; } = true;

        public ErrorReturn(IEnumerable<object> errors) => Errors = errors;
    }
}