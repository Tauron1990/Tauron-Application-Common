using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tauron.Application.CQRS.Common.Dto;
using Tauron.Application.CQRS.Common.Dto.Persistable;
using Tauron.Application.CQRS.Common.Server;
using Tauron.Application.CQRS.Dispatcher.Core;
using Tauron.Application.CQRS.Dispatcher.EventStore;
using Tauron.Application.CQRS.Dispatcher.EventStore.Data;

namespace Tauron.Application.CQRS.Dispatcher.Controllers
{
    [ApiController]
    [Route("Api/[controller]")]
    public class DispatcherController : ControllerBase
    {
        private readonly IApiKeyStore _apiKeyStore;
        private readonly DispatcherDatabaseContext _context;
        private readonly IConnectionManager _connectionManager;

        public DispatcherController(IApiKeyStore apiKeyStore, DispatcherDatabaseContext context, IConnectionManager connectionManager)
        {
            _apiKeyStore = apiKeyStore;
            _context = context;
            _connectionManager = connectionManager;
        }

        [HttpGet]
        public async Task<ActionResult<ObjectStade>> Get([FromBody] ApiObjectId id)
        {
            if (!(await _apiKeyStore.Validate(id.ApiKey)).Ok)
                return base.Forbid();

            string realId = id.Id;

            var entity = await _context.ObjectStades.AsNoTracking().FirstOrDefaultAsync(o => o.Identifer == realId);

            return entity == null
                ? new ObjectStade {Identifer = realId}
                : new ObjectStade
                {
                    Data = entity.Data, // TypeFactory.Create(entity.OriginType, entity.Data) as IObjectData,
                    Identifer = entity.Identifer,
                    //OriginalType = entity.OriginType
                };
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] ApiObjectStade stade)
        {
            if (!(await _apiKeyStore.Validate(stade.ApiKey)).Ok)
                return base.Forbid();

            string id = stade.ObjectStade.Identifer;

            var entity = await _context.ObjectStades.FirstOrDefaultAsync(o => o.Identifer == id);

            if (entity != null)
                entity.Data = stade.ObjectStade.Data;
            else
            {
                _context.ObjectStades.Add(new ObjectStadeEntity
                                          {
                                              Data = stade.ObjectStade.Data.ToString(),
                                              Identifer = stade.ObjectStade.Identifer,
                                              //OriginType = stade.ObjectStade.OriginalType
                                          });
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        [Route("Events")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DomainMessage>>> GetEvents([FromBody]EventsRequest eventId)
        {
            if (!(await _apiKeyStore.Validate(eventId.ApiKey)).Ok) return Forbid();

            return _context.EventEntities.Where(ee => ee.Id == eventId.Guid).Where(ee => ee.Version > eventId.Version)
                .Select(ee => ee.ToDomainMessage()).ToList();
        }

        [Route("Hub")]
        [HttpPut]
        public async Task<IActionResult> Validate([FromBody] ValidateConnection validateConnection)
        {
            var (ok, serviceName) = await _apiKeyStore.Validate(validateConnection.ApiKey);
            if (!ok)
                return Forbid();

            await _connectionManager.Validated(validateConnection.NewId, serviceName, validateConnection.OldId);
            return Ok();
        }
    }
}