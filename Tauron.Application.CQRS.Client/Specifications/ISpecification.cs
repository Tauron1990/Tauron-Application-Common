using System.Threading.Tasks;
using Tauron.Application.CQRS.Client.Infrastructure;

namespace Tauron.Application.CQRS.Client.Specifications
{
    public interface ISpecification
    {
        Task<OperationResult> IsSatisfiedBy(object obj);
    }
}