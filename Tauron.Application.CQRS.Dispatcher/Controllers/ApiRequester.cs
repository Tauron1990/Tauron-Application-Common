using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Tauron.Application.CQRS.Dispatcher.Core;

namespace Tauron.Application.CQRS.Dispatcher.Controllers
{
    [ApiController]
    [Route("Api/[controller]")]
    public class ApiRequesterController : ControllerBase
    {
        private readonly IApiKeyStore _keyStore;
        private readonly IConfiguration _configuration;

        public ApiRequesterController(IApiKeyStore keyStore, IConfiguration configuration)
        {
            _keyStore = keyStore;
            _configuration = configuration;
        }

        [Route(nameof(RegisterApiKey))]
        [HttpGet]
        public async Task<ActionResult<string>> RegisterApiKey(string serviceName)
        {
            if (!_configuration.GetValue<bool>("FreeAcess")) return Forbid();

            return await _keyStore.Register(serviceName);
        }

        [Route(nameof(RemoveApiKey))]
        [HttpGet]
        public async Task<ActionResult<bool>> RemoveApiKey(string serviceName)
        {
            if (!_configuration.GetValue<bool>("FreeAcess")) return Forbid();

            return await _keyStore.Remove(serviceName);
        }
    }
}