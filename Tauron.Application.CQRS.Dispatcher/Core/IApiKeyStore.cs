using System.Threading.Tasks;

namespace Tauron.Application.CQRS.Dispatcher.Core
{
    public interface IApiKeyStore
    {
        //Task<string> GetServiceFromKey(string apiKey);

        Task<(bool Ok, string ServiceName)> Validate(string? apiKey);

        Task<string?> Register(string name);
        Task<bool> Remove(string serviceName);
    }
}