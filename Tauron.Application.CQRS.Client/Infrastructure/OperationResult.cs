using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Tauron.Application.CQRS.Client.Infrastructure
{
    public sealed class OperationResult : IMessage
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

        public List<OperationError> Errors { get; set; }

        private OperationResult(bool error, IEnumerable<OperationError> errors)
        {
            Error = error;
            Errors = errors.ToList();
        }

        public OperationResult()
        {
            Errors = new List<OperationError>();
        }
    }
}