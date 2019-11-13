using Microsoft.AspNetCore.Mvc;
using Tauron.Application.CQRS.Dispatcher.Core;

namespace Tauron.Application.CQRS.Dispatcher.Controllers
{
    public class HealthInfo
    {
        public int CurrentClients { get; set; }

        public int PendingMessages { get; set; }

        public HealthInfo()
        {
            
        }

        public HealthInfo(int currentClients, int pendingMessages)
        {
            CurrentClients = currentClients;
            PendingMessages = pendingMessages;
        }
    }

    [Route("/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        private readonly IConnectionManager _connectionManager;

        public HealthController(IConnectionManager connectionManager) 
            => _connectionManager = connectionManager;

        [HttpGet]
        public JsonResult Get() 
            => new JsonResult(new HealthInfo(_connectionManager.GetCurrentClients(), _connectionManager.GetPendingMessages()));
    }
}