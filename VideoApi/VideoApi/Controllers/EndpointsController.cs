using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Mappings;
using VideoApi.Services.Contracts;
using VideoApi.Services.Mappers;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.Controllers
{
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("conferences")]
    [ApiController]
    public class EndpointsController : ControllerBase
    {
        private readonly IQueryHandler _queryHandler;
        private readonly ICommandHandler _commandHandler;
        private readonly IVideoPlatformService _videoPlatformService;
        private readonly ILogger<EndpointsController> _logger;

        public EndpointsController(IQueryHandler queryHandler, 
            ICommandHandler commandHandler, 
            IVideoPlatformService videoPlatformService,
            ILogger<EndpointsController> logger)
        {
            _queryHandler = queryHandler;
            _logger = logger;
            _commandHandler = commandHandler;
            _videoPlatformService = videoPlatformService;
        }

        /// <summary>
        /// Get all endpoints for a conference
        /// </summary>
        /// <param name="conferenceId">Id of the conference</param>
        /// <returns>List of endpoints</returns>
        [HttpGet("{conferenceId}/endpoints")]
        [OpenApiOperation("GetEndpointsForConference")]
        [ProducesResponseType(typeof(IList<EndpointResponse>), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> GetEndpointsForConference(Guid conferenceId)
        {
            _logger.LogDebug("Retrieving endpoints for conference {conferenceId}", conferenceId);
            var query = new GetEndpointsForConferenceQuery(conferenceId);
            var endpoints = await _queryHandler.Handle<GetEndpointsForConferenceQuery, IList<Endpoint>>(query);
            var response = endpoints.Select(EndpointToResponseMapper.MapEndpointResponse).ToList();
            return Ok(response);
        }

        /// <summary>
        /// Add an endpoint to a conference
        /// </summary>
        /// <param name="conferenceId">Id of conference</param>
        /// <param name="request">Endpoint details</param>
        [HttpPost("{conferenceId}/endpoints")]
        [OpenApiOperation("AddEndpointToConference")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public async Task<IActionResult> AddEndpointToConference([FromRoute] Guid conferenceId,
            [FromBody] AddEndpointRequest request)
        {
            _logger.LogDebug("Attempting to add endpoint {DisplayName} to conference", request.DisplayName);
            
            var command = new AddEndpointCommand(conferenceId, request.DisplayName, request.SipAddress, request.Pin, request.DefenceAdvocate);
            await _commandHandler.Handle(command);

            var conference = await _queryHandler.Handle<GetConferenceByIdQuery, Conference>(new GetConferenceByIdQuery(conferenceId));
            var endpointDtos = conference.GetEndpoints().Select(EndpointMapper.MapToEndpoint);
            await _videoPlatformService.UpdateVirtualCourtRoomAsync(conference.Id, conference.AudioRecordingRequired, endpointDtos);
            
            _logger.LogDebug("Successfully added endpoint {DisplayName} to conference", request.DisplayName);
            return NoContent();
        }

        /// <summary>
        /// Remove an endpoint from a conference
        /// </summary>
        /// <param name="conferenceId"></param>
        /// <param name="sipAddress"></param>
        /// <returns></returns>
        [HttpDelete("{conferenceId}/endpoints/{sipAddress}")]
        [OpenApiOperation("RemoveEndpointFromConference")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public async Task<IActionResult> RemoveEndpointFromConference(Guid conferenceId, string sipAddress)
        {
            _logger.LogDebug("Attempting to remove endpoint {sipAddress} from conference", sipAddress);

            var command = new RemoveEndpointCommand(conferenceId, sipAddress);
            await _commandHandler.Handle(command);

            var conference = await _queryHandler.Handle<GetConferenceByIdQuery, Conference>(new GetConferenceByIdQuery(conferenceId));
            var endpointDtos = conference.GetEndpoints().Select(EndpointMapper.MapToEndpoint);
            await _videoPlatformService.UpdateVirtualCourtRoomAsync(conference.Id, conference.AudioRecordingRequired, endpointDtos);
            
            _logger.LogDebug("Successfully removed endpoint {sipAddress} from conference", sipAddress);
            return NoContent();
        }

        /// <summary>
        /// Update an endpoint's display name or assign the defence advocate
        /// </summary>
        /// <param name="conferenceId">the conference id</param>
        /// <param name="sipAddress">the endpoint sip address to be updated</param>
        /// <param name="request">the updated values of an endpoint</param>
        /// <returns>an OK status</returns>
        [HttpPatch("{conferenceId}/endpoints/{sipAddress}")]
        [OpenApiOperation("UpdateDisplayNameForEndpoint")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateDisplayNameForEndpointAsync(Guid conferenceId, string sipAddress,
            [FromBody] UpdateEndpointRequest request)
        {
            _logger.LogDebug(
                "Attempting to update endpoint {sipAddress} with display name {DisplayName}", sipAddress, request.DisplayName);

            var command = new UpdateEndpointCommand(conferenceId, sipAddress, request.DisplayName, request.DefenceAdvocate);
            await _commandHandler.Handle(command);

            if (!string.IsNullOrWhiteSpace(request.DisplayName))
            {
                await UpdateDisplayNameInKinly(conferenceId);
            }

            _logger.LogDebug(
                "Successfully updated endpoint {sipAddress} with display name {DisplayName}", sipAddress, request.DisplayName);
            return Ok();
        }

        private async Task UpdateDisplayNameInKinly(Guid conferenceId)
        {
            var conference =
                await _queryHandler.Handle<GetConferenceByIdQuery, Conference>(
                    new GetConferenceByIdQuery(conferenceId));
            var endpointDtos = conference.GetEndpoints().Select(EndpointMapper.MapToEndpoint);
            await _videoPlatformService.UpdateVirtualCourtRoomAsync(conference.Id,
                conference.AudioRecordingRequired,
                endpointDtos);
        }
    }
}
