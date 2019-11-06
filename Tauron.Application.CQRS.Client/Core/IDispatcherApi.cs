using System.Threading.Tasks;
using RestEase;
using Tauron.Application.CQRS.Common.Dto;
using Tauron.Application.CQRS.Common.Dto.Persistable;
using Tauron.Application.CQRS.Common.Server;

namespace Tauron.Application.CQRS.Client.Core
{
    public interface IDispatcherApi
    {
        [Put("Hub")]
        Task Validate([Body] ValidateConnection validateConnection);

        [Get("Events")]
        Task<DomainMessage[]> GetEvents([Body]EventsRequest request);

        [Get]
        Task<ObjectStade> Get([Body] ApiObjectId id);

        [Put]
        Task Put([Body] ApiObjectStade stade);
    }
}