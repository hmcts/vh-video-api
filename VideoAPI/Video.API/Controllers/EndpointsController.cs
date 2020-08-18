using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using VideoApi.Contract.Responses;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;

namespace Video.API.Controllers
{
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("conferences")]
    [ApiController]
    public class EndpointsController : ControllerBase
    {
        private readonly IQueryHandler _queryHandler;
        private readonly ILogger<EndpointsController> _logger;

        public EndpointsController(IQueryHandler queryHandler, ILogger<EndpointsController> logger)
        {
            _queryHandler = queryHandler;
            _logger = logger;
        }

        [HttpGet("{conferenceId}/endpoints")]
        [SwaggerOperation(OperationId = "GetEndpointsForConference")]
        [ProducesResponseType(typeof(IList<EndpointResponse>), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> GetEndpointsForConference(Guid conferenceId)
        {
            _logger.LogDebug($"Retrieving endpoints for conference {conferenceId}");
            var query = new GetEndpointsForConferenceQuery(conferenceId);
            var endpoints = await _queryHandler.Handle<GetEndpointsForConferenceQuery, IList<Endpoint>>(query);

            return Ok(endpoints);
        }
    }
}
