using System;

namespace Tauron.Application.CQRS.Common.Dto.Persistable
{
    public class EventsRequest
    {
        public string ApiKey { get; set; }

        public Guid Guid { get; set; }

        public long Version { get; set; }

        public EventsRequest()
        {
            
        }

        public EventsRequest(string apiKey, Guid guid, long version)
        {
            ApiKey = apiKey;
            Guid = guid;
            Version = version;
        }
    }
}