using Tauron.Application.CQRS.Common.Dto.Persistable;

namespace Tauron.Application.CQRS.Common.Dto
{
    public class ApiObjectStade
    {
        public string ApiKey { get; set; }

        public ObjectStade ObjectStade { get; set; }
    }
}