using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
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
        private readonly ICommandHandler _commandHandler;
        private readonly ILogger<EndpointsController> _logger;

        public EndpointsController(IQueryHandler queryHandler, ICommandHandler commandHandler,
            ILogger<EndpointsController> logger)
        {
            _queryHandler = queryHandler;
            _logger = logger;
            _commandHandler = commandHandler;
        }

        /// <summary>
        /// Get all endpoints for a conference
        /// </summary>
        /// <param name="conferenceId">Id of the conference</param>
        /// <returns>List of endpoints</returns>
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

        /// <summary>
        /// Add an endpoint to a conference
        /// </summary>
        /// <param name="conferenceId">Id of conference</param>
        /// <param name="request">Endpoint details</param>
        [HttpPost("{conferenceId}/endpoints")]
        [SwaggerOperation(OperationId = "AddEndpointToConference")]
        [ProducesResponseType(typeof(IList<EndpointResponse>), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> AddEndpointToConference([FromRoute] Guid conferenceId,
            [FromBody] AddEndpointRequest request)
        {
            _logger.LogDebug($"Attempting to add endpoint {request.DisplayName} to conference {conferenceId}");

            var command = new AddEndpointCommand(conferenceId, request.DisplayName, request.SipAddress, request.Pin);
            await _commandHandler.Handle(command);

            _logger.LogDebug($"Successfully added endpoint {request.DisplayName} to conference {conferenceId}");
            return NoContent();
        }

        [HttpPost("{conferenceId}/endpoints/{endpointId}")]
        [SwaggerOperation(OperationId = "RemoveEndpointFromConference")]
        [ProducesResponseType(typeof(IList<EndpointResponse>), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> RemoveEndpointFromConference(Guid conferenceId, Guid endpointId)
        {
            return Accepted();
        }
    }
}
