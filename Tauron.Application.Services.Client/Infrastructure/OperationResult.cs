using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Tauron.Application.Services.Client.Infrastructure
{
    public sealed class OperationResult
    {
        public static OperationResult Success 
            => new OperationResult(false, ImmutableArray<OperationError>.Empty);

        public static OperationResult Failed(IEnumerable<OperationError> errors)
            => new OperationResult(true, ImmutableArray.CreateRange(errors));

        public static OperationResult Failed(params OperationError[] errors)
            => new OperationResult(true, ImmutableArray.CreateRange(errors));

        public OperationResult Merge(OperationResult result)
        {
            if (Error || result.Error)
                return Failed(Errors.Concat(result.Errors));

            return Success;
        }

        public bool Error { get; set; }

        public IEnumerable<OperationError> Errors { get; set; }

        private OperationResult(bool error, IEnumerable<OperationError> errors)
        {
            Error = error;
            Errors = errors;
        }

        public OperationResult()
        {
            
        }
    }
}