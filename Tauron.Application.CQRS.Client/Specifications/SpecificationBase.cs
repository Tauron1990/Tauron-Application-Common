using System.Threading.Tasks;
using Tauron.Application.CQRS.Common.Dto;

namespace Tauron.Application.CQRS.Client.Specifications
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