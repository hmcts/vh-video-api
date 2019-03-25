using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Video.API.Extensions;
using Video.API.Mappings;
using Video.API.Validations;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;

namespace Video.API.Controllers
{
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("conferences")]
    [ApiController]
    public class ConferenceController : ControllerBase
    {
        private readonly IQueryHandler _queryHandler;
        private readonly ICommandHandler _commandHandler;

        public ConferenceController(IQueryHandler queryHandler, ICommandHandler commandHandler)
        {
            _queryHandler = queryHandler;
            _commandHandler = commandHandler;
        }

        /// <summary>
        /// Request to book a conference
        /// </summary>
        /// <param name="request">Details of a conference</param>
        /// <returns>Details of the new conference</returns>
        [HttpPost]
        [SwaggerOperation(OperationId = "BookNewConference")]
        [ProducesResponseType(typeof(ConferenceDetailsResponse), (int) HttpStatusCode.Created)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> BookNewConference(BookNewConferenceRequest request)
        {
            var result = await new BookNewConferenceRequestValidation().ValidateAsync(request);
            if (!result.IsValid)
            {
                ModelState.AddFluentValidationErrors(result.Errors);
                return BadRequest(ModelState);
            }
            
            var participants = request.Participants.Select(x =>
                    new Participant(x.ParticipantRefId, x.Name, x.DisplayName, x.Username, x.UserRole,
                        x.CaseTypeGroup))
                .ToList();
            var createConferenceCommand = new CreateConferenceCommand(request.HearingRefId, request.CaseType,
                request.ScheduledDateTime, request.CaseNumber, request.CaseName, request.ScheduledDuration, participants);
            await _commandHandler.Handle(createConferenceCommand);
            
            var conferenceId = createConferenceCommand.NewConferenceId;
            var getConferenceByIdQuery = new GetConferenceByIdQuery(conferenceId);
            var queriedConference =
                await _queryHandler.Handle<GetConferenceByIdQuery, Conference>(getConferenceByIdQuery);

            var mapper = new ConferenceToDetailsResponseMapper();
            var response = mapper.MapConferenceToResponse(queriedConference);
            return CreatedAtAction(nameof(GetConferenceDetailsById), new {conferenceId = response.Id}, response);
        }

        /// <summary>
        /// Update the conference status
        /// </summary>
        /// <param name="conferenceId">The id of the conference to update</param>
        /// <param name="request">New status for the conference</param>
        /// <returns></returns>
        [HttpPatch("{conferenceId}")]
        [SwaggerOperation(OperationId = "UpdateConferenceStatus")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateConferenceStatus(Guid conferenceId,
            UpdateConferenceStatusRequest request)
        {
            if (conferenceId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(conferenceId), $"Please provide a valid {nameof(conferenceId)}");
                return BadRequest(ModelState);
            }
            
            var result = await new UpdateConferenceStatusRequestValidation().ValidateAsync(request);
            if (!result.IsValid)
            {
                ModelState.AddFluentValidationErrors(result.Errors);
                return BadRequest(ModelState);
            }
            
            var getConferenceByIdQuery = new GetConferenceByIdQuery(conferenceId);
            var queriedConference =
                await _queryHandler.Handle<GetConferenceByIdQuery, Conference>(getConferenceByIdQuery);

            if (queriedConference == null)
            {
                return NotFound();
            }
            
            var command = new UpdateConferenceStatusCommand(conferenceId, request.State);
            await _commandHandler.Handle(command);
            return NoContent();
        }

        /// <summary>
        /// Get the details of a conference
        /// </summary>
        /// <param name="conferenceId">Id of the conference</param>
        /// <returns>Full details including participants and statuses of a conference</returns>
        [HttpGet("{conferenceId}")]
        [SwaggerOperation(OperationId = "GetConferenceDetailsById")]
        [ProducesResponseType(typeof(ConferenceDetailsResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetConferenceDetailsById(Guid conferenceId)
        {
            if (conferenceId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(conferenceId), $"Please provide a valid {nameof(conferenceId)}");
                return BadRequest(ModelState);
            }
            
            var getConferenceByIdQuery = new GetConferenceByIdQuery(conferenceId);
            var queriedConference =
                await _queryHandler.Handle<GetConferenceByIdQuery, Conference>(getConferenceByIdQuery);

            if (queriedConference == null)
            {
                return NotFound();
            }
            var mapper = new ConferenceToDetailsResponseMapper();
            var response = mapper.MapConferenceToResponse(queriedConference);
            return Ok(response);
        }
        
        /// <summary>
        /// Remove an existing conference
        /// </summary>
        /// <param name="conferenceId">The hearing id</param>
        /// <returns></returns>
        [HttpDelete("{conferenceId}")]
        [SwaggerOperation(OperationId = "RemoveConference")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> RemoveHearing(Guid conferenceId)
        {
            if (conferenceId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(conferenceId), $"Please provide a valid {nameof(conferenceId)}");
                return BadRequest(ModelState);
            }
            
            var removeConferenceCommand = new RemoveConferenceCommand(conferenceId);
            try
            {
                await _commandHandler.Handle(removeConferenceCommand);
            }
            catch (ConferenceNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }

        /// <summary>
        /// Get non-closed conferences for a participant by their username
        /// </summary>
        /// <param name="username">person username</param>
        /// <returns>Hearing details</returns>
        [HttpGet(Name = "GetConferencesForUsername")]
        [SwaggerOperation(OperationId = "GetConferencesForUsername")]
        [ProducesResponseType(typeof(List<ConferenceSummaryResponse>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetConferencesForUsername([FromQuery] string username)
        {
            if (!username.IsValidEmail())
            {
                ModelState.AddModelError(nameof(username), $"Please provide a valid {nameof(username)}");
                return BadRequest(ModelState);
            }

            var query = new GetConferencesByUsernameQuery(username);
            var conferences = await _queryHandler.Handle<GetConferencesByUsernameQuery, List<Conference>>(query);

            var mapper = new ConferenceToSummaryResponseMapper();
            var response = conferences.Select(mapper.MapConferenceToSummaryResponse);
            return Ok(response);
        }
        
        /// <summary>
        /// Get conferences by hearing ref id
        /// </summary>
        /// <param name="hearingRefId">Hearing ID</param>
        /// <returns>Full details including participants and statuses of a conference</returns>
        [HttpGet("hearings/{hearingRefId}")]
        [SwaggerOperation(OperationId = "GetConferenceByHearingRefId")]
        [ProducesResponseType(typeof(ConferenceDetailsResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetConferenceByHearingRefId(Guid hearingRefId)
        {
            if (hearingRefId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(hearingRefId), $"Please provide a valid {nameof(hearingRefId)}");
                return BadRequest(ModelState);
            }

            var query = new GetConferenceByHearingRefIdQuery(hearingRefId);
            var conference = await _queryHandler.Handle<GetConferenceByHearingRefIdQuery, Conference>(query);

            if (conference == null)
            {
                return NotFound();
            }
            
            var mapper = new ConferenceToDetailsResponseMapper();
            var response = mapper.MapConferenceToResponse(conference);
            return Ok(response);
        }
        
    }
}