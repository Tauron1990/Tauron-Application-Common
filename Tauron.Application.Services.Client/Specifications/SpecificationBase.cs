using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tauron.Application.Services.Client.Infrastructure;

namespace Tauron.Application.Services.Client.Specifications
{
    public abstract class SpecificationBase<TTarget> : ISpecification
    {
        public async Task<OperationResult> IsSatisfiedBy(object obj)
        {
            if (obj is TTarget target)
                return await IsSatisfiedBy(target);

            return OperationResult.Failed(OperationError.Error(0, "Incompatiple Type"));
        }

        protected abstract Task<OperationResult> IsSatisfiedBy(TTarget target);
    }
}