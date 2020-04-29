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
using VideoApi.Services.Contracts;
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
        private readonly ServicesConfiguration _servicesConfiguration;
        private readonly ILogger<ConferenceController> _logger;
        private readonly IAudioPlatformService _audioPlatformService;


        public ConferenceController(IQueryHandler queryHandler, ICommandHandler commandHandler,
            IVideoPlatformService videoPlatformService, IOptions<ServicesConfiguration> servicesConfiguration,
            ILogger<ConferenceController> logger, IAudioPlatformService audioPlatformService)
        {
            _queryHandler = queryHandler;
            _commandHandler = commandHandler;
            _videoPlatformService = videoPlatformService;
            _servicesConfiguration = servicesConfiguration.Value;
            _logger = logger;
            _audioPlatformService = audioPlatformService;
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
        public async Task<IActionResult> BookNewConferenceAsync(BookNewConferenceRequest request)
        {
            _logger.LogDebug("BookNewConference");
            
            foreach (var participant in request.Participants)
            {
                participant.Username = participant.Username.ToLower().Trim();
                participant.Name = participant.Name.Trim();
                participant.DisplayName = participant.DisplayName.Trim();
            }

            string ingestUrl = null;
            
            if (request.AudioRecordingRequired)
            {
                var createAudioRecordingResponse = await _audioPlatformService.CreateAudioApplicationWithStreamAsync
                (
                    request.HearingRefId
                );

                ingestUrl = createAudioRecordingResponse.IngestUrl;
                
                if (!createAudioRecordingResponse.Success)
                {
                    _logger.LogWarning($"Error creating audio recording for caseNumber: {request.CaseNumber} and hearingId: {request.HearingRefId}");    
                }
            }
            
            var conferenceId = await CreateConferenceAsync(request, ingestUrl);
            _logger.LogDebug("Conference Created");
            
            await BookKinlyMeetingRoomAsync(conferenceId, request.AudioRecordingRequired, ingestUrl);
            _logger.LogDebug("Kinly Room Booked");
            
            var getConferenceByIdQuery = new GetConferenceByIdQuery(conferenceId);
            var queriedConference = await _queryHandler.Handle<GetConferenceByIdQuery, Conference>(getConferenceByIdQuery);

            var response = ConferenceToDetailsResponseMapper.MapConferenceToResponse(queriedConference, _servicesConfiguration.PexipSelfTestNode);
            
            _logger.LogInformation($"Created conference {response.Id} for hearing {request.HearingRefId}");
            
            return CreatedAtAction(nameof(GetConferenceDetailsByIdAsync), new {conferenceId = response.Id}, response);
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
        public async Task<IActionResult> UpdateConferenceAsync(UpdateConferenceRequest request)
        {
            try
            {
                var command = new UpdateConferenceDetailsCommand(request.HearingRefId, request.CaseNumber,
                    request.CaseType, request.CaseName, request.ScheduledDuration, request.ScheduledDateTime,
                    request.HearingVenueName, request.AudioRecordingRequired);

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
        public async Task<IActionResult> GetConferenceDetailsByIdAsync(Guid conferenceId)
        {
            _logger.LogDebug($"GetConferenceDetailsById {conferenceId}");
            
            var getConferenceByIdQuery = new GetConferenceByIdQuery(conferenceId);
            var queriedConference = await _queryHandler.Handle<GetConferenceByIdQuery, Conference>(getConferenceByIdQuery);

            if (queriedConference == null)
            {
                _logger.LogWarning($"Unable to find conference {conferenceId}");
                
                return NotFound();
            }
            
            var response = ConferenceToDetailsResponseMapper.MapConferenceToResponse(queriedConference, _servicesConfiguration.PexipSelfTestNode);
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
        public async Task<IActionResult> RemoveConferenceAsync(Guid conferenceId)
        {
            _logger.LogDebug($"RemoveConference {conferenceId}");
            var removeConferenceCommand = new RemoveConferenceCommand(conferenceId);
            try
            {
                await _commandHandler.Handle(removeConferenceCommand);
                await SafelyRemoveCourtRoomAsync(conferenceId);
                
                _logger.LogInformation($"Successfully removed conference {conferenceId}");
                
                return NoContent();
            }
            catch (ConferenceNotFoundException ex)
            {
                _logger.LogError(ex, $"Unable to find conference {conferenceId}");
                
                return NotFound();
            }
        }

        /// <summary>
        /// Get todays conferences
        /// </summary>
        /// <returns>Conference details</returns>
        [HttpGet("today/vho")]
        [SwaggerOperation(OperationId = "GetConferencesTodayForAdmin")]
        [ProducesResponseType(typeof(List<ConferenceForAdminResponse>), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> GetConferencesTodayForAdminByUsernameAsync(
            [FromQuery] ConferenceForAdminRequest request)
        {
            _logger.LogDebug("GetConferencesTodayForAdmin");
            
            var query = new GetConferencesTodayForAdminQuery
            {
                VenueNames = request.VenueNames
            };
            
            var conferences = await _queryHandler.Handle<GetConferencesTodayForAdminQuery, List<Conference>>(query);

            var response = conferences.Select(ConferenceForAdminResponseMapper.MapConferenceToSummaryResponse);
            
            return Ok(response);
        }

        /// <summary>
        /// Get all conferences for a judge
        /// </summary>
        /// <param name="username">judge username</param>
        /// <returns>List of conferences for judge</returns>
        [HttpGet("today/judge")]
        [SwaggerOperation(OperationId = "GetConferencesTodayForJudgeByUsername")]
        [ProducesResponseType(typeof(List<ConferenceForJudgeResponse>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetConferencesTodayForJudgeByUsernameAsync([FromQuery] string username)
        {
            _logger.LogDebug($"GetConferencesTodayForJudgeByUsername {username}");
            
            if (!username.IsValidEmail())
            {
                ModelState.AddModelError(nameof(username), $"Please provide a valid {nameof(username)}");
                
                _logger.LogWarning($"Invalid username {username}");
                
                return BadRequest(ModelState);
            }

            var query = new GetConferencesForTodayByJudgeQuery(username.ToLower().Trim());
            var conferences = await _queryHandler.Handle<GetConferencesForTodayByJudgeQuery, List<Conference>>(query);
            var response = conferences.Select(ConferenceForJudgeResponseMapper.MapConferenceSummaryToModel);
            
            return Ok(response);
        }

        /// <summary>
        /// Get non-closed conferences for a participant by their username
        /// </summary>
        /// <param name="username">person username</param>
        /// <returns>List of non-closed conferences for judge</returns>
        [HttpGet("today/individual")]
        [SwaggerOperation(OperationId = "GetConferencesTodayForIndividualByUsername")]
        [ProducesResponseType(typeof(List<ConferenceForIndividualResponse>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetConferencesTodayForIndividualByUsernameAsync([FromQuery] string username)
        {
            _logger.LogDebug($"GetConferencesTodayForIndividualByUsername {username}");
            
            if (!username.IsValidEmail())
            {
                ModelState.AddModelError(nameof(username), $"Please provide a valid {nameof(username)}");
                
                _logger.LogWarning($"Invalid username {username}");
                
                return BadRequest(ModelState);
            }

            var query = new GetConferencesForTodayByIndividualQuery(username.ToLower().Trim());
            var conferences = await _queryHandler.Handle<GetConferencesForTodayByIndividualQuery, List<Conference>>(query);
            var response = conferences.Select(ConferenceForIndividualResponseMapper.MapConferenceSummaryToModel);

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
        public async Task<IActionResult> GetConferenceByHearingRefIdAsync(Guid hearingRefId)
        {
            _logger.LogDebug($"GetConferenceByHearingRefId {hearingRefId}");
            
            var query = new GetConferenceByHearingRefIdQuery(hearingRefId);
            var conference = await _queryHandler.Handle<GetConferenceByHearingRefIdQuery, Conference>(query);

            if (conference == null)
            {
                _logger.LogWarning($"Unable to find conference with hearing id {hearingRefId}");
                
                return NotFound();
            }
            
            var response = ConferenceToDetailsResponseMapper.MapConferenceToResponse(conference, _servicesConfiguration.PexipSelfTestNode);
            
            return Ok(response);
        }
        
        /// <summary>
        /// Get list of expired conferences 
        /// </summary>
        /// <returns>Conference summary details</returns>
        
        [HttpGet("expired")]
        [SwaggerOperation(OperationId = "GetExpiredOpenConferences")]
        [ProducesResponseType(typeof(List<ExpiredConferencesResponse>), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> GetExpiredOpenConferencesAsync()
        {
            _logger.LogDebug("GetExpiredOpenConferences");
            
            var query = new GetExpiredUnclosedConferencesQuery();
            var conferences = await _queryHandler.Handle<GetExpiredUnclosedConferencesQuery, List<Conference>>(query);
            var response = conferences.Select(ConferenceToExpiredConferenceMapper.MapConferenceToExpiredResponse);
            
            return Ok(response);
        }

        /// <summary>
        /// Close a conference, set its state to closed
        /// </summary>
        /// <param name="conferenceId">conference id</param>
        /// <returns>No Content status</returns>
        [HttpPut("{conferenceId}/close")]
        [SwaggerOperation(OperationId = "CloseConference")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CloseConferenceAsync(Guid conferenceId)
        {
            try
            {
                var command = new CloseConferenceCommand(conferenceId);

                await _commandHandler.Handle(command);
                await SafelyRemoveCourtRoomAsync(conferenceId);
                await DeleteAudioRecordingApplication(conferenceId);
                
                return NoContent();
            }
            catch (ConferenceNotFoundException ex)
            {
                _logger.LogError(ex, $"Unable to find conference {conferenceId}");
                
                return NotFound();
            }
        }
        
        /// <summary>
        /// Get today's conferences where judges are in hearings
        /// </summary>
        /// <returns>Conference details</returns>
        [HttpGet("today/judgesinhearings")]
        [SwaggerOperation(OperationId = "GetJudgesInHearingsToday")]
        [ProducesResponseType(typeof(List<JudgeInHearingResponse>), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> GetJudgesInHearingsTodayAsync()
        {
            _logger.LogDebug("GetJudgesInHearingsToday");

            var conferences = await _queryHandler.Handle<GetJudgesInHearingsTodayQuery, List<Conference>>(new GetJudgesInHearingsTodayQuery());

            var response = conferences.SelectMany(ConferenceForJudgeResponseMapper.MapConferenceSummaryToJudgeInHearingResponse);
            
            return Ok(response);
        }

        private async Task SafelyRemoveCourtRoomAsync(Guid conferenceId)
        {
            var meetingRoom = await _videoPlatformService.GetVirtualCourtRoomAsync(conferenceId);
            if (meetingRoom != null)
            {
                await _videoPlatformService.DeleteVirtualCourtRoomAsync(conferenceId);
            }
        }

        private async Task DeleteAudioRecordingApplication(Guid conferenceId)
        {
            var getConferenceByIdQuery = new GetConferenceByIdQuery(conferenceId);
            var queriedConference = await _queryHandler.Handle<GetConferenceByIdQuery, Conference>(getConferenceByIdQuery);

            if (queriedConference != null && queriedConference.AudioRecordingRequired)
            {
                await _audioPlatformService.DeleteAudioApplicationAsync(queriedConference.HearingRefId);
            }
        }

        private async Task BookKinlyMeetingRoomAsync(Guid conferenceId, bool audioRecordingRequired, string ingestUrl)
        {
            MeetingRoom meetingRoom;
            try
            {
                meetingRoom = await _videoPlatformService.BookVirtualCourtroomAsync(conferenceId, audioRecordingRequired, ingestUrl);            
            }
            catch (DoubleBookingException ex)
            {
                _logger.LogError(ex, $"Kinly Room already booked for conference {conferenceId}");
                
                meetingRoom = await _videoPlatformService.GetVirtualCourtRoomAsync(conferenceId);
            }

            if (meetingRoom == null) return;
            
            var command = new UpdateMeetingRoomCommand
            (
                conferenceId, meetingRoom.AdminUri, meetingRoom.JudgeUri, meetingRoom.ParticipantUri, meetingRoom.PexipNode
            );
            
            await _commandHandler.Handle(command);
        }

        private async Task<Guid> CreateConferenceAsync(BookNewConferenceRequest request, string ingestUrl)
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
            
            var createConferenceCommand = new CreateConferenceCommand
            (
                request.HearingRefId, request.CaseType, request.ScheduledDateTime, request.CaseNumber, 
                request.CaseName, request.ScheduledDuration, participants, request.HearingVenueName, request.AudioRecordingRequired, ingestUrl
            );
            
            await _commandHandler.Handle(createConferenceCommand);
            
            return createConferenceCommand.NewConferenceId;
        }
    }
}
