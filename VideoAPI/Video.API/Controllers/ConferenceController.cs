using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Video.API.Extensions;
using Video.API.Validations;
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

            var createConferenceCommand = new CreateConferenceCommand(request.HearingRefId, request.CaseType,
                request.ScheduledDateTime, request.CaseNumber);
            await _commandHandler.Handle(createConferenceCommand);

            var conferenceId = createConferenceCommand.NewConferenceId;
            var participants = request.Participants.Select(x =>
                    new Participant(x.ParticipantRefId, x.Name, x.DisplayName, x.Username, x.HearingRole,
                        x.CaseTypeGroup))
                .ToList();

            var addParticipantCommand = new AddParticipantsToConferenceCommand(conferenceId, participants);
            await _commandHandler.Handle(addParticipantCommand);

            var getConferenceByIdQuery = new GetConferenceByIdQuery(conferenceId);
            var queriedConference =
                await _queryHandler.Handle<GetConferenceByIdQuery, Conference>(getConferenceByIdQuery);

            var response = MapConferenceToResponse(queriedConference);
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
            
            var response = MapConferenceToResponse(queriedConference);
            return Ok(response);
        }
        
        private ConferenceDetailsResponse MapConferenceToResponse(Conference conference)
        {
            var response = new ConferenceDetailsResponse
            {
                Id = conference.Id,
                CaseType = conference.CaseType,
                CaseNumber = conference.CaseNumber,
                ScheduledDateTime = conference.ScheduledDateTime,
                Statuses = MapConferenceStatusToResponse(conference.GetConferenceStatuses()),
                Participants = MapParticipantsToResponse(conference.GetParticipants())
            };
            return response;
        }

        private List<ParticipantDetailsResponse> MapParticipantsToResponse(IEnumerable<Participant> participants)
        {
            var response = new List<ParticipantDetailsResponse>();
            foreach (var participant in participants)
            {
                var paResponse = new ParticipantDetailsResponse
                {
                    Id = participant.Id,
                    Name = participant.Name,
                    Username = participant.Username,
                    DisplayName = participant.DisplayName,
                    HearingRole = participant.HearingRole,
                    CaseTypeGroup = participant.CaseTypeGroup,
                    Statuses = MapParticipantStatusToResponse(participant.GetParticipantStatuses())
                };
                response.Add(paResponse);
            }

            return response;
        }

        private List<ParticipantStatusResponse> MapParticipantStatusToResponse(
            IEnumerable<ParticipantStatus> participantStatuses)
        {
            var statusResponse = participantStatuses.Select(x => new ParticipantStatusResponse
            {
                ParticipantState = x.ParticipantState,
                TimeStamp = x.TimeStamp
            }).ToList();
            return statusResponse;
        }

        private List<ConferenceStatusResponse> MapConferenceStatusToResponse(
            IEnumerable<ConferenceStatus> conferenceStatuses)
        {
            return conferenceStatuses.Select(x => new ConferenceStatusResponse
            {
                ConferenceState = x.ConferenceState,
                TimeStamp = x.TimeStamp
            }).ToList();
        }
    }
}