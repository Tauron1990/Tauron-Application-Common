using JetBrains.Annotations;

namespace Tauron.Application.CQRS.Common.Configuration
{
    public class ServerConfiguration
    {
        public string ConnectionString { get; private set; } = string.Empty;

        public bool Memory { get; set; }

        [PublicAPI]
        public ServerConfiguration WithDatabase(string connectionString)
        {
            ConnectionString = connectionString;

            return this;
        }
    }
}