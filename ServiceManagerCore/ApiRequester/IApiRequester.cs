using System.Threading.Tasks;
using RestEase;

namespace ServiceManager.Core.ApiRequester
{
    public interface IApiRequester
    {
        [Get(nameof(RegisterApiKey))]
        Task<string> RegisterApiKey(string serviceName);
    }
}