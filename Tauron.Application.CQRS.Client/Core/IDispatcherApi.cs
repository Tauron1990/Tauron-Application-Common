using System.Threading.Tasks;
using RestEase;
using Tauron.Application.CQRS.Common.Dto;
using Tauron.Application.CQRS.Common.Dto.Persistable;
using Tauron.Application.CQRS.Common.Server;

namespace Tauron.Application.CQRS.Client.Core
{
    public interface IDispatcherApi
    {
        [Delete("Services")]
        Task RemoveService(string name);

        [Get("Hub")]
        Task<bool> Validate([Body] ValidateConnection validateConnection);

        [Get("Events")]
        Task<DomainMessage[]> GetEvents([Body]EventsRequest request);

        [Get]
        Task<ObjectStade> Get([Body] ApiObjectId id);

        [Put]
        Task Put([Body] ApiObjectStade stade);
    }
}