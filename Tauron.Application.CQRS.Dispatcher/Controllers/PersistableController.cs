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
    public class PersistableController : ControllerBase
    {
        private readonly IApiKeyStore _apiKeyStore;
        private readonly DispatcherDatabaseContext _context;

        public PersistableController(IApiKeyStore apiKeyStore, DispatcherDatabaseContext context)
        {
            _apiKeyStore = apiKeyStore;
            _context = context;
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
                .Select(ee => new DomainMessage
                {
                    EventData = ee.Data,
                    EventName = ee.EventName,
                    EventType = ee.EventType,
                    SequenceNumber = ee.SequenceNumber,
                    Id = ee.Id.Value,
                    TimeStamp = ee.TimeStamp,
                    Version = ee.Version,
                    TypeName = ee.OriginType
                }).ToList();
        }
    }
}