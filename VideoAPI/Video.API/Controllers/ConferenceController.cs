using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
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
using VideoApi.Services;
using VideoApi.Services.Exceptions;
using Task = System.Threading.Tasks.Task;

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
        private readonly IVideoPlatformService _videoPlatformService;

        public ConferenceController(IQueryHandler queryHandler, ICommandHandler commandHandler,
            IVideoPlatformService videoPlatformService)
        {
            _queryHandler = queryHandler;
            _commandHandler = commandHandler;
            _videoPlatformService = videoPlatformService;
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
            foreach (var participant in request.Participants)
            {
                participant.Username = participant.Username.ToLower().Trim();
                participant.Name = participant.Name.Trim();
                participant.DisplayName = participant.DisplayName.Trim();
            }
            
            var conferenceId = await CreateConference(request);
            await BookKinlyMeetingRoom(conferenceId);
            
            var getConferenceByIdQuery = new GetConferenceByIdQuery(conferenceId);
            var queriedConference =
                await _queryHandler.Handle<GetConferenceByIdQuery, Conference>(getConferenceByIdQuery);

            var mapper = new ConferenceToDetailsResponseMapper();
            var response = mapper.MapConferenceToResponse(queriedConference);
            return CreatedAtAction(nameof(GetConferenceDetailsById), new {conferenceId = response.Id}, response);
        }

        private async Task BookKinlyMeetingRoom(Guid conferenceId)
        {
            MeetingRoom meetingRoom;
            try
            {
                meetingRoom = await _videoPlatformService.BookVirtualCourtroomAsync(conferenceId);            
            }
            catch (DoubleBookingException)
            {
                meetingRoom = await _videoPlatformService.GetVirtualCourtRoomAsync(conferenceId);
            }

            if (meetingRoom == null) return;
            
            var command = new UpdateMeetingRoomCommand(conferenceId, meetingRoom.AdminUri, meetingRoom.JudgeUri,
                meetingRoom.ParticipantUri, meetingRoom.PexipNode);
            await _commandHandler.Handle(command);
        }

        private async Task<Guid> CreateConference(BookNewConferenceRequest request)
        {
            var existingConference = await _queryHandler.Handle<GetConferenceByHearingRefIdQuery, Conference>(
                new GetConferenceByHearingRefIdQuery(request.HearingRefId));

            if (existingConference != null && !existingConference.IsClosed()) return existingConference.Id;
            
            var participants = request.Participants.Select(x =>
                    new Participant(x.ParticipantRefId, x.Name, x.DisplayName, x.Username, x.UserRole,
                        x.CaseTypeGroup))
                .ToList();
            var createConferenceCommand = new CreateConferenceCommand(request.HearingRefId, request.CaseType,
                request.ScheduledDateTime, request.CaseNumber, request.CaseName, request.ScheduledDuration, participants);
            await _commandHandler.Handle(createConferenceCommand);
            return createConferenceCommand.NewConferenceId;

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
        /// <param name="conferenceId">The conference id</param>
        /// <returns></returns>
        [HttpDelete("{conferenceId}")]
        [SwaggerOperation(OperationId = "RemoveConference")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> RemoveConference(Guid conferenceId)
        {
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
        /// <returns>Conference details</returns>
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

            var query = new GetConferencesByUsernameQuery(username.ToLower().Trim());
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