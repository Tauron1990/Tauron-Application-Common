using System.Threading.Tasks;
using Tauron.Application.CQRS.Common.Dto;

namespace Tauron.Application.CQRS.Client.Specifications
{
    public interface ISpecification
    {
        Task<OperationResult> IsSatisfiedBy(object obj);
    }
}