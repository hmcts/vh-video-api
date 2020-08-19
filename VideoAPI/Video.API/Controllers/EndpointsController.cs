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
        /// Update the display name of an endpoint of a conference
        /// </summary>
        /// <param name="conferenceId">the conference id</param>
        /// <param name="endpointId">the endpoint id to be updated</param>
        /// <param name="request">the display name to be updated</param>
        /// <returns>an OK status</returns>
        [HttpPut("{conferenceId}/endpoints/{endpointId}/displayname")]
        [SwaggerOperation(OperationId = "UpdateEndpointForConference")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateEndpointForConference(Guid conferenceId, Guid endpointId, 
            [FromBody] UpdateEndpointRequest request)
        {
            _logger.LogDebug($"Attempting to update endpoint {endpointId} for conference {conferenceId} with displayname {request.DisplayName}");

            var command = new UpdateEndpointCommand(conferenceId, endpointId, request.DisplayName);
            await _commandHandler.Handle(command);

            _logger.LogDebug($"Successfully updated endpoint {endpointId} from conference {conferenceId} with displayname {request.DisplayName}");
            return Ok();
        }
    }
}
