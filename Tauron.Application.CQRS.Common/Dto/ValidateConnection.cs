namespace Tauron.Application.CQRS.Common.Dto
{
    public class ValidateConnection
    {
        public string? NewId { get; set; }

        public string? OldId { get; set; }

        public string? ApiKey { get; set; }

        public ValidateConnection()
        {
            
        }

        public ValidateConnection(string newId, string oldId, string apiKey)
        {
            NewId = newId;
            OldId = oldId;
            ApiKey = apiKey;
        }
    }
}