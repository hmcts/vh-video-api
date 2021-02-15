using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSwag.Annotations;
using VideoApi.Common.Security.Kinly;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.DTOs;
using VideoApi.DAL.Exceptions;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Extensions;
using VideoApi.Factories;
using VideoApi.Mappings;
using VideoApi.Services.Contracts;
using VideoApi.Services.Dtos;
using VideoApi.Services.Exceptions;
using VideoApi.Services.Mappers;
using VideoApi.Validations;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.Controllers
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
        private readonly KinlyConfiguration _kinlyConfiguration;
        private readonly ILogger<ConferenceController> _logger;
        private readonly IAudioPlatformService _audioPlatformService;
        private readonly IAzureStorageServiceFactory _azureStorageServiceFactory;
        private readonly IPollyRetryService _pollyRetryService;


        public ConferenceController(IQueryHandler queryHandler, ICommandHandler commandHandler,
            IVideoPlatformService videoPlatformService, IOptions<KinlyConfiguration> kinlyConfiguration, 
            ILogger<ConferenceController> logger, IAudioPlatformService audioPlatformService, 
            IAzureStorageServiceFactory azureStorageServiceFactory, IPollyRetryService pollyRetryService)
        {
            _queryHandler = queryHandler;
            _commandHandler = commandHandler;
            _videoPlatformService = videoPlatformService;
            _kinlyConfiguration = kinlyConfiguration.Value;
            _logger = logger;
            _audioPlatformService = audioPlatformService;
            _azureStorageServiceFactory = azureStorageServiceFactory;
            _pollyRetryService = pollyRetryService;
        }

        /// <summary>
        /// Request to book a conference
        /// </summary>
        /// <param name="request">Details of a conference</param>
        /// <returns>Details of the new conference</returns>
        [HttpPost]
        [OpenApiOperation("BookNewConference")]
        [ProducesResponseType(typeof(ConferenceDetailsResponse), (int) HttpStatusCode.Created)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> BookNewConferenceAsync(BookNewConferenceRequest request)
        {
            _logger.LogDebug("BookNewConference");

            foreach (var participant in request.Participants)
            {
                participant.Username = participant.Username.ToLower().Trim();
                participant.Name = participant.Name.Trim();
                participant.FirstName = participant.FirstName.Trim();
                participant.LastName = participant.LastName.Trim();
                participant.DisplayName = participant.DisplayName.Trim();
            }

            var createAudioRecordingResponse = await CreateAudioApplicationWithRetryAsync(request);

            if (!createAudioRecordingResponse.Success)
            {
                return StatusCode((int) createAudioRecordingResponse.StatusCode, createAudioRecordingResponse.Message);
            }

            var conferenceId = await CreateConferenceWithRetiesAsync(request, createAudioRecordingResponse.IngestUrl);
            _logger.LogDebug("Conference Created");

            var conferenceEndpoints =
                await _queryHandler.Handle<GetEndpointsForConferenceQuery, IList<Endpoint>>(
                    new GetEndpointsForConferenceQuery(conferenceId));
            var endpointDtos = conferenceEndpoints.Select(EndpointMapper.MapToEndpoint);

            var kinlyBookedSuccess = await BookKinlyMeetingRoomWithRetriesAsync(conferenceId, request.AudioRecordingRequired,
                createAudioRecordingResponse.IngestUrl, endpointDtos);
            
            if (!kinlyBookedSuccess)
            {
                var message = $"Could not book and find kinly meeting room for conferenceId: {conferenceId}";
                _logger.LogError(message);
                return StatusCode((int) HttpStatusCode.InternalServerError, message);
            }
            
            _logger.LogDebug("Kinly Room Booked");

            var getConferenceByIdQuery = new GetConferenceByIdQuery(conferenceId);
            var queriedConference =
                await _queryHandler.Handle<GetConferenceByIdQuery, Conference>(getConferenceByIdQuery);

            var response =
                ConferenceToDetailsResponseMapper.MapConferenceToResponse(queriedConference,
                    _kinlyConfiguration.PexipSelfTestNode);

            _logger.LogInformation("Created conference {ResponseId} for hearing {HearingRefId}", response.Id, request.HearingRefId);

            return CreatedAtAction("GetConferenceDetailsById", new {conferenceId = response.Id}, response);
        }

        /// <summary>
        /// Updates a conference
        /// </summary>
        /// <param name="request">Details of a conference</param>
        /// <returns>Ok status</returns>
        [HttpPut]
        [OpenApiOperation("UpdateConference")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateConferenceAsync(UpdateConferenceRequest request)
        {
            _logger.LogDebug("UpdateConference");

            var query = new GetNonClosedConferenceByHearingRefIdQuery(request.HearingRefId);
            var conference = await _queryHandler.Handle<GetNonClosedConferenceByHearingRefIdQuery, Conference>(query);

            if (conference == null)
            {
                _logger.LogWarning("Unable to find conference with hearing id {HearingRefId}", request.HearingRefId);

                return NotFound();
            }

            var endpointDtos = conference.GetEndpoints().Select(EndpointMapper.MapToEndpoint);
            await _videoPlatformService.UpdateVirtualCourtRoomAsync(conference.Id, request.AudioRecordingRequired,
                endpointDtos);

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
        [OpenApiOperation("GetConferenceDetailsById")]
        [ProducesResponseType(typeof(ConferenceDetailsResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetConferenceDetailsByIdAsync(Guid conferenceId)
        {
            _logger.LogDebug("GetConferenceDetailsById {ConferenceId}", conferenceId);

            var getConferenceByIdQuery = new GetConferenceByIdQuery(conferenceId);
            var queriedConference =
                await _queryHandler.Handle<GetConferenceByIdQuery, Conference>(getConferenceByIdQuery);

            if (queriedConference == null)
            {
                _logger.LogWarning("Unable to find conference {ConferenceId}", conferenceId);

                return NotFound();
            }

            var response =
                ConferenceToDetailsResponseMapper.MapConferenceToResponse(queriedConference,
                    _kinlyConfiguration.PexipSelfTestNode);
            return Ok(response);
        }

        /// <summary>
        /// Remove an existing conference
        /// </summary>
        /// <param name="conferenceId">The conference id</param>
        /// <returns></returns>
        [HttpDelete("{conferenceId}")]
        [OpenApiOperation("RemoveConference")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> RemoveConferenceAsync(Guid conferenceId)
        {
            _logger.LogDebug("RemoveConference {ConferenceId}", conferenceId);
            var removeConferenceCommand = new RemoveConferenceCommand(conferenceId);
            try
            {
                await _commandHandler.Handle(removeConferenceCommand);
                await SafelyRemoveCourtRoomAsync(conferenceId);

                _logger.LogInformation("Successfully removed conference {ConferenceId}", conferenceId);

                return NoContent();
            }
            catch (ConferenceNotFoundException ex)
            {
                _logger.LogError(ex, "Unable to find conference {ConferenceId}", conferenceId);

                return NotFound();
            }
        }

        /// <summary>
        /// Get todays conferences
        /// </summary>
        /// <returns>Conference details</returns>
        [HttpGet("today/vho")]
        [OpenApiOperation("GetConferencesTodayForAdmin")]
        [ProducesResponseType(typeof(List<ConferenceForAdminResponse>), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> GetConferencesTodayForAdminByUsernameAsync(
            [FromQuery] ConferenceForAdminRequest request)
        {
            _logger.LogDebug("GetConferencesTodayForAdmin");

            var query = new GetConferencesTodayForAdminQuery
            {
                UserNames = request.UserNames
            };

            var conferences = await _queryHandler.Handle<GetConferencesTodayForAdminQuery, List<Conference>>(query);

            var response = conferences.Select(c => ConferenceForAdminResponseMapper.MapConferenceToSummaryResponse(c , _kinlyConfiguration));

            return Ok(response);
        }

        /// <summary>
        /// Get all conferences for a judge
        /// </summary>
        /// <param name="username">judge username</param>
        /// <returns>List of conferences for judge</returns>
        [HttpGet("today/judge")]
        [OpenApiOperation("GetConferencesTodayForJudgeByUsername")]
        [ProducesResponseType(typeof(List<ConferenceForJudgeResponse>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetConferencesTodayForJudgeByUsernameAsync([FromQuery] string username)
        {
            _logger.LogDebug("GetConferencesTodayForJudgeByUsername {Username}", username);

            if (!username.IsValidEmail())
            {
                ModelState.AddModelError(nameof(username), $"Please provide a valid {nameof(username)}");

                _logger.LogWarning("Invalid username {Username}", username);

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
        [OpenApiOperation("GetConferencesTodayForIndividualByUsername")]
        [ProducesResponseType(typeof(List<ConferenceForIndividualResponse>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetConferencesTodayForIndividualByUsernameAsync([FromQuery] string username)
        {
            _logger.LogDebug("GetConferencesTodayForIndividualByUsername {Username}", username);

            if (!username.IsValidEmail())
            {
                ModelState.AddModelError(nameof(username), $"Please provide a valid {nameof(username)}");

                _logger.LogWarning("Invalid username {Username}", username);

                return BadRequest(ModelState);
            }

            var query = new GetConferencesForTodayByIndividualQuery(username.ToLower().Trim());
            var conferences =
                await _queryHandler.Handle<GetConferencesForTodayByIndividualQuery, List<Conference>>(query);
            var response = conferences.Select(ConferenceForIndividualResponseMapper.MapConferenceSummaryToModel);

            return Ok(response);
        }

        /// <summary>
        /// Get conferences by hearing ref id
        /// </summary>
        /// <param name="hearingRefId">Hearing ID</param>
        /// <param name="includeClosed">Include closed conferences in search</param>
        /// <returns>Full details including participants and statuses of a conference</returns>
        [HttpGet("hearings/{hearingRefId}")]
        [OpenApiOperation("GetConferenceByHearingRefId")]
        [ProducesResponseType(typeof(ConferenceDetailsResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetConferenceByHearingRefIdAsync(Guid hearingRefId, [FromQuery]bool? includeClosed = false)
        {
            _logger.LogDebug("GetConferenceByHearingRefId {HearingRefId}", hearingRefId);

            var query = new GetNonClosedConferenceByHearingRefIdQuery(hearingRefId, includeClosed.GetValueOrDefault());
            var conference = await _queryHandler.Handle<GetNonClosedConferenceByHearingRefIdQuery, Conference>(query);

            if (conference == null)
            {
                _logger.LogWarning("Unable to find conference with hearing id {HearingRefId}", hearingRefId);

                return NotFound();
            }

            var response =
                ConferenceToDetailsResponseMapper.MapConferenceToResponse(conference,
                    _kinlyConfiguration.PexipSelfTestNode);

            return Ok(response);
        }

        /// <summary>
        /// Get list of expired conferences 
        /// </summary>
        /// <returns>Conference summary details</returns>

        [HttpGet("expired")]
        [OpenApiOperation("GetExpiredOpenConferences")]
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
        /// Get list of expired conferences that have audiorecording option on
        /// </summary>
        /// <returns>List of expired conferences</returns>
        [HttpGet("audiorecording/expired")]
        [OpenApiOperation("GetExpiredAudiorecordingConferences")]
        [ProducesResponseType(typeof(List<ExpiredConferencesResponse>), (int) HttpStatusCode.OK)]

        public async Task<IActionResult> GetExpiredAudiorecordingConferencesAsync()
        {
            _logger.LogDebug("GetExpiredAudiorecordingConferences");
            var query = new GetExpiredAudiorecordingConferencesQuery();
            var conferences =
                await _queryHandler.Handle<GetExpiredAudiorecordingConferencesQuery, List<Conference>>(query);
            var response = conferences.Select(ConferenceToExpiredConferenceMapper.MapConferenceToExpiredResponse);

            return Ok(response);

        }

        /// <summary>
        /// Close a conference, set its state to closed
        /// </summary>
        /// <param name="conferenceId">conference id</param>
        /// <returns>No Content status</returns>
        [HttpPut("{conferenceId}/close")]
        [OpenApiOperation("CloseConference")]
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
                _logger.LogError(ex, "Unable to find conference {conferenceId}", conferenceId);

                return NotFound();
            }
        }

        /// <summary>
        /// Get today's conferences where judges are in hearings
        /// </summary>
        /// <returns>Conference details</returns>
        [HttpGet("today/judgesinhearings")]
        [OpenApiOperation("GetJudgesInHearingsToday")]
        [ProducesResponseType(typeof(List<JudgeInHearingResponse>), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> GetJudgesInHearingsTodayAsync()
        {
            _logger.LogDebug("GetJudgesInHearingsToday");

            var conferences =
                await _queryHandler.Handle<GetJudgesInHearingsTodayQuery, List<Conference>>(
                    new GetJudgesInHearingsTodayQuery());

            var response =
                conferences.SelectMany(ConferenceForJudgeResponseMapper.MapConferenceSummaryToJudgeInHearingResponse);

            return Ok(response);
        }

        /// <summary>
        /// Anonymises the Conference and Participant data.
        /// </summary>
        /// <returns></returns>
        [HttpPatch("anonymiseconferences")]
        [OpenApiOperation("AnonymiseConferences")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public async Task<IActionResult> AnonymiseConferencesAsync()
        {
            _logger.LogDebug("AnonymiseConferencesAndParticipantInformation");

            var anonymiseConferenceCommand = new AnonymiseConferencesCommand();
            await _commandHandler.Handle(anonymiseConferenceCommand);

            _logger.LogInformation("Records updated: {RecordsUpdated}", anonymiseConferenceCommand.RecordsUpdated);
            return NoContent();
        }

        [HttpDelete("expiredHearbeats")]
        [OpenApiOperation("RemoveHeartbeatsForConferences")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public async Task<IActionResult> RemoveHeartbeatsForConferencesAsync()
        {
            _logger.LogDebug("Remove heartbeats for conferences over 14 days old.");

            var removeHeartbeatsCommand = new RemoveHeartbeatsForConferencesCommand();
            await _commandHandler.Handle(removeHeartbeatsCommand);

            _logger.LogInformation($"Successfully removed heartbeats for conferences");
            return NoContent();
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
            var queriedConference =
                await _queryHandler.Handle<GetConferenceByIdQuery, Conference>(getConferenceByIdQuery);

            if (queriedConference != null && queriedConference.AudioRecordingRequired)
            {
                try
                {
                    await EnsureAudioFileExists(queriedConference);
                    await _audioPlatformService.DeleteAudioApplicationAsync(queriedConference.HearingRefId);
                }
                catch (AudioPlatformFileNotFoundException ex)
                {
                    _logger.LogError(ex, ex.Message);
                }

            }
        }

        private async Task EnsureAudioFileExists(Conference conference)
        {
            var azureStorageService = _azureStorageServiceFactory.Create(AzureStorageServiceType.Vh);
            var allBlobs = await azureStorageService.GetAllBlobNamesByFilePathPrefix(conference.HearingRefId.ToString());

            if (!allBlobs.Any() && conference.ActualStartTime.HasValue)
            {
                var msg = $"Audio recording file not found for hearing: {conference.HearingRefId}";
                throw new AudioPlatformFileNotFoundException(msg, HttpStatusCode.NotFound);
            }
        }
   
        public async Task<bool> BookKinlyMeetingRoomAsync(Guid conferenceId,
            bool audioRecordingRequired,
            string ingestUrl,
            IEnumerable<EndpointDto> endpoints)
        {
            MeetingRoom meetingRoom;
            try
            {
                meetingRoom = await _videoPlatformService.BookVirtualCourtroomAsync(conferenceId,
                    audioRecordingRequired,
                    ingestUrl,
                    endpoints);
            }
            catch (DoubleBookingException ex)
            {
                _logger.LogError(ex, "Kinly Room already booked for conference {conferenceId}", conferenceId);

                meetingRoom = await _videoPlatformService.GetVirtualCourtRoomAsync(conferenceId);
            }

            if (meetingRoom == null)  return false;

            var command = new UpdateMeetingRoomCommand
            (
                conferenceId, meetingRoom.AdminUri, meetingRoom.JudgeUri, meetingRoom.ParticipantUri,
                meetingRoom.PexipNode, meetingRoom.TelephoneConferenceId
            );

            await _commandHandler.Handle(command);

            return true;
        }
        
        private async Task<Guid> CreateConferenceAsync(BookNewConferenceRequest request, string ingestUrl)
        {
            var existingConference = await _queryHandler.Handle<CheckConferenceOpenQuery, Conference>(
                new CheckConferenceOpenQuery(request.ScheduledDateTime, request.CaseNumber, request.CaseName));

            if (existingConference != null) return existingConference.Id;

            var participants = request.Participants.Select(x =>
                    new Participant(x.ParticipantRefId, x.Name, x.FirstName, x.LastName, x.DisplayName, x.Username,
                        x.UserRole.MapToDomainEnum(), x.HearingRole, x.CaseTypeGroup, x.ContactEmail, x.ContactTelephone)
                    {
                        Representee = x.Representee
                    })
                .ToList();

            var endpoints = request.Endpoints
                .Select(x => new Endpoint(x.DisplayName, x.SipAddress, x.Pin, x.DefenceAdvocate)).ToList();

            var linkedParticipants = request.Participants
                .SelectMany(x => x.LinkedParticipants)
                .Select(x => new LinkedParticipantDto()
                {
                    ParticipantRefId = x.ParticipantRefId, 
                    LinkedRefId = x.LinkedRefId, 
                    Type = x.Type.MapToDomainEnum()
                }).ToList();
            
            var createConferenceCommand = new CreateConferenceCommand
            (
                request.HearingRefId, request.CaseType, request.ScheduledDateTime, request.CaseNumber,
                request.CaseName, request.ScheduledDuration, participants, request.HearingVenueName,
                request.AudioRecordingRequired, ingestUrl, endpoints, linkedParticipants
            );

            await _commandHandler.Handle(createConferenceCommand);

            return createConferenceCommand.NewConferenceId;
        }

        private async Task<bool> BookKinlyMeetingRoomWithRetriesAsync(Guid conferenceId,
            bool audioRecordingRequired,
            string ingestUrl,
            IEnumerable<EndpointDto> endpoints)
        {
            var result = await _pollyRetryService.WaitAndRetryAsync<Exception, bool>
            (
                3,
                _ => TimeSpan.FromSeconds(10),
                retryAttempt => _logger.LogWarning($"Failed to BookKinlyMeetingRoomAsync. Retrying attempt {retryAttempt}"),
                callResult => !callResult,
                () => BookKinlyMeetingRoomAsync(conferenceId, audioRecordingRequired, ingestUrl, endpoints)
            );

            return result;
        }

        private async Task<Guid> CreateConferenceWithRetiesAsync(BookNewConferenceRequest request, string ingestUrl)
        {
            var result = await _pollyRetryService.WaitAndRetryAsync<Exception, Guid>
            (
                3,
                _ => TimeSpan.FromSeconds(10),
                retryAttempt => _logger.LogWarning("Failed to CreateConferenceAsync. Retrying attempt {RetryAttempt}", retryAttempt),
                callResult => callResult == Guid.Empty,
                async () => await CreateConferenceAsync(request, ingestUrl));

            return result;
        }
        

        private async Task<AudioPlatformServiceResponse> CreateAudioApplicationWithRetryAsync(BookNewConferenceRequest request)
        {
            var result = await _pollyRetryService.WaitAndRetryAsync<Exception, AudioPlatformServiceResponse>
            (
                3,
                _ => TimeSpan.FromSeconds(10),
                retryAttempt => _logger.LogWarning("Failed to CreateAudioApplicationAsync. Retrying attempt {RetryAttempt}", retryAttempt),
                callResult => callResult == null || !callResult.Success,
                () => _audioPlatformService.CreateAudioApplicationAsync(request.HearingRefId)
            );

            return result;
        }
    }
}
