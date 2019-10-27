using System.Collections.Generic;
using System.Threading.Tasks;
using Tauron.Application.Services.Client.Infrastructure;

namespace Tauron.Application.Services.Client.Specifications
{
    public interface ISpecification
    {
        Task<OperationResult> IsSatisfiedBy(object obj);
    }
}