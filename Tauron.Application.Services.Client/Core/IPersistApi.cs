using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestEase;
using Tauron.Application.CQRS.Common.Dto;
using Tauron.Application.CQRS.Common.Dto.Persistable;
using Tauron.Application.CQRS.Common.Server;

namespace Tauron.Application.Services.Client.Core
{
    public interface IPersistApi
    {
        [Get("Events")]
        Task<DomainMessage[]> GetEvents([Body]EventsRequest request);

        [Get]
        Task<ObjectStade> Get([Body] ApiObjectId id);

        [Put]
        Task Put([Body] ApiObjectStade stade);
    }
}