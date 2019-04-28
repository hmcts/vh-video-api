using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Net;
using System.Threading.Tasks;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;

namespace Video.API.Controllers
{
    [Produces("application/json")]
    [Route("HealthCheck")]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        private readonly IQueryHandler _queryHandler;

        public HealthCheckController(IQueryHandler queryHandler)
        {
            _queryHandler = queryHandler;
        }

        /// <summary>
        /// Check Service Health
        /// </summary>
        /// <returns>Error if fails, otherwise OK status</returns>
        [HttpGet("health")]
        [SwaggerOperation(OperationId = "CheckServiceHealth")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Health()
        {
            try
            {
                var hearingId = Guid.NewGuid();
                var query = new GetConferenceByIdQuery(hearingId);
                await _queryHandler.Handle<GetConferenceByIdQuery, Conference>(query);
            }
            catch (Exception ex)
            {
                var data = new
                {
                    ex.Message,
                    ex.Data
                };
                return StatusCode((int)HttpStatusCode.InternalServerError, data);
            }

            return Ok();
        }
    }
}