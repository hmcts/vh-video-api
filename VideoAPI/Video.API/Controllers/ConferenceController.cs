using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Annotations;
using Video.API.Mappings;
using Video.API.Validations;
using VideoApi.Common.Configuration;
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
        private readonly ILogger<ConferenceController> _logger;
        private readonly ServicesConfiguration _servicesConfiguration;

        public ConferenceController(IQueryHandler queryHandler, ICommandHandler commandHandler,
            IVideoPlatformService videoPlatformService, IOptions<ServicesConfiguration> servicesConfiguration,
            ILogger<ConferenceController> logger)
        {
            _queryHandler = queryHandler;
            _commandHandler = commandHandler;
            _videoPlatformService = videoPlatformService;
            _logger = logger;
            _servicesConfiguration = servicesConfiguration.Value;
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
            _logger.LogDebug("BookNewConference");
            foreach (var participant in request.Participants)
            {
                participant.Username = participant.Username.ToLower().Trim();
                participant.Name = participant.Name.Trim();
                participant.DisplayName = participant.DisplayName.Trim();
            }
            
            var conferenceId = await CreateConference(request);
            _logger.LogDebug("Conference Created");
            await BookKinlyMeetingRoom(conferenceId);
            _logger.LogDebug("Kinly Room Booked");
            
            var getConferenceByIdQuery = new GetConferenceByIdQuery(conferenceId);
            var queriedConference =
                await _queryHandler.Handle<GetConferenceByIdQuery, Conference>(getConferenceByIdQuery);

            var mapper = new ConferenceToDetailsResponseMapper();
            var response = mapper.MapConferenceToResponse(queriedConference, _servicesConfiguration.PexipSelfTestNode);
            _logger.LogInformation($"Created conference {response.Id} for hearing {request.HearingRefId}");
            return CreatedAtAction(nameof(GetConferenceDetailsById), new {conferenceId = response.Id}, response);
        }

        /// <summary>
        /// Updates a conference
        /// </summary>
        /// <param name="request">Details of a conference</param>
        /// <returns>Ok status</returns>
        [HttpPut]
        [SwaggerOperation(OperationId = "UpdateConference")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateConference(UpdateConferenceRequest request)
        {
            try
            {
                var command = new UpdateConferenceDetailsCommand(request.HearingRefId, request.CaseNumber,
                    request.CaseType, request.CaseName, request.ScheduledDuration, request.ScheduledDateTime);

                await _commandHandler.Handle(command);
                return Ok();
            }
            catch (ConferenceNotFoundException)
            {
                return NotFound();
            }
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
            _logger.LogDebug($"GetConferenceDetailsById {conferenceId}");
            var getConferenceByIdQuery = new GetConferenceByIdQuery(conferenceId);
            var queriedConference =
                await _queryHandler.Handle<GetConferenceByIdQuery, Conference>(getConferenceByIdQuery);

            if (queriedConference == null)
            {
                _logger.LogError($"Unable to find conference {conferenceId}");
                return NotFound();
            }
            var mapper = new ConferenceToDetailsResponseMapper();
            var response = mapper.MapConferenceToResponse(queriedConference, _servicesConfiguration.PexipSelfTestNode);
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
            _logger.LogDebug($"RemoveConference {conferenceId}");
            var removeConferenceCommand = new RemoveConferenceCommand(conferenceId);
            try
            {
                await _commandHandler.Handle(removeConferenceCommand);
            }
            catch (ConferenceNotFoundException)
            {
                _logger.LogError($"Unable to find conference {conferenceId}");
                return NotFound();
            }

            _logger.LogInformation($"Successfully removed conference {conferenceId}");
            return NoContent();
        }

        /// <summary>
        /// Get todays conferences
        /// </summary>
        /// <returns>Conference details</returns>
        [HttpGet("today")]
        [SwaggerOperation(OperationId = "GetConferencesToday")]
        [ProducesResponseType(typeof(List<ConferenceSummaryResponse>), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> GetConferencesToday()
        {
            _logger.LogDebug("GetConferencesToday");
            var query = new GetConferencesTodayQuery();
            var conferences = await _queryHandler.Handle<GetConferencesTodayQuery, List<Conference>>(query);

            var mapper = new ConferenceToSummaryResponseMapper();
            var response = conferences.Select(mapper.MapConferenceToSummaryResponse);
            return Ok(response);
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
            _logger.LogDebug($"GetConferencesForUsername {username}");
            if (!username.IsValidEmail())
            {
                ModelState.AddModelError(nameof(username), $"Please provide a valid {nameof(username)}");
                _logger.LogError($"Invalid username {username}");
                return BadRequest(ModelState);
            }

            var query = new GetConferencesForTodayByUsernameQuery(username.ToLower().Trim());
            var conferences = await _queryHandler.Handle<GetConferencesForTodayByUsernameQuery, List<Conference>>(query);

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
            _logger.LogDebug($"GetConferenceByHearingRefId {hearingRefId}");
            var query = new GetConferenceByHearingRefIdQuery(hearingRefId);
            var conference = await _queryHandler.Handle<GetConferenceByHearingRefIdQuery, Conference>(query);

            if (conference == null)
            {
                _logger.LogError($"Unable to find conference with hearing id {hearingRefId}");
                return NotFound();
            }
            
            var mapper = new ConferenceToDetailsResponseMapper();
            var response = mapper.MapConferenceToResponse(conference, _servicesConfiguration.PexipSelfTestNode);
            return Ok(response);
        }
        
        /// <summary>
        /// Get conferences where the scheduledDate is lower or equal to the scheduled date time and which are open. i.e. not in the state 'closed'
        /// </summary>
        /// <param name="scheduledDate">The conference scheduled date time</param>
        /// <returns>Conference summary details</returns>
        [HttpGet("fromdate")]
        [SwaggerOperation(OperationId = "GetOpenConferencesByScheduledDate")]
        [ProducesResponseType(typeof(List<ConferenceSummaryResponse>), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> GetOpenConferencesByScheduledDate([FromQuery] DateTime scheduledDate)
        {
            _logger.LogDebug("GetOpenConferencesByScheduledDate");
            
            var query = new GetOpenConferencesByDateTimeQuery(scheduledDate);
            var conferences = await _queryHandler.Handle<GetOpenConferencesByDateTimeQuery, List<Conference>>(query);

            var mapper = new ConferenceToSummaryResponseMapper();
            var response = conferences.Select(mapper.MapConferenceToSummaryResponse);
            
            return Ok(response);
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
                _logger.LogWarning($"Kinly Room already booked for conference {conferenceId}");
                meetingRoom = await _videoPlatformService.GetVirtualCourtRoomAsync(conferenceId);
            }

            if (meetingRoom == null) return;
            
            var command = new UpdateMeetingRoomCommand(conferenceId, meetingRoom.AdminUri, meetingRoom.JudgeUri,
                meetingRoom.ParticipantUri, meetingRoom.PexipNode);
            await _commandHandler.Handle(command);
        }

        private async Task<Guid> CreateConference(BookNewConferenceRequest request)
        {
            var existingConference = await _queryHandler.Handle<CheckConferenceOpenQuery, Conference>(
                new CheckConferenceOpenQuery(request.ScheduledDateTime, request.CaseNumber, request.CaseName));

            if (existingConference != null) return existingConference.Id;
            
            var participants = request.Participants.Select(x =>
                    new Participant(x.ParticipantRefId, x.Name, x.DisplayName, x.Username, x.UserRole,
                        x.CaseTypeGroup)
                    {
                        Representee = x.Representee
                    })
                .ToList();
            var createConferenceCommand = new CreateConferenceCommand(request.HearingRefId, request.CaseType,
                request.ScheduledDateTime, request.CaseNumber, request.CaseName, request.ScheduledDuration, participants);
            await _commandHandler.Handle(createConferenceCommand);
            return createConferenceCommand.NewConferenceId;

        }
    }
}