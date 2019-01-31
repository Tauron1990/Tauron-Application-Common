using System;
using System.Collections.Generic;

namespace Tauron.Application.Common.BaseLayer.BusinessLayer
{
    public class CallErrorException : Exception
    {
        public CallErrorException(IEnumerable<object> error) => Error = error;
        public IEnumerable<object> Error { get; }
    }
}