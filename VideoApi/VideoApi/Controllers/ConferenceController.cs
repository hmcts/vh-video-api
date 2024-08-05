using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using VideoApi.Common.Security.Supplier.Base;
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
using VideoApi.Services.Factories;
using VideoApi.Mappings;
using VideoApi.Services;
using VideoApi.Services.Contracts;
using VideoApi.Services.Dtos;
using VideoApi.Services.Exceptions;
using VideoApi.Services.Mappers;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.Controllers
{
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("conferences")]
    [ApiController]
    [SuppressMessage("Info Code Smell", "S1133:Deprecated code should be removed")]
    public class ConferenceController : ControllerBase
    {
        private readonly IQueryHandler _queryHandler;
        private readonly ICommandHandler _commandHandler;
        private readonly IVideoPlatformService _videoPlatformService;
        private readonly SupplierConfiguration _supplierConfiguration;
        private readonly ILogger<ConferenceController> _logger;
        private readonly IAudioPlatformService _audioPlatformService;
        private readonly IAzureStorageServiceFactory _azureStorageServiceFactory;
        private readonly IPollyRetryService _pollyRetryService;
        private readonly IBackgroundWorkerQueue _backgroundWorkerQueue;
        private readonly IFeatureToggles _featureToggles;

        public ConferenceController(IQueryHandler queryHandler, 
            ICommandHandler commandHandler,
            IVideoPlatformService videoPlatformService, 
            ISupplierApiSelector supplierLocator, 
            ILogger<ConferenceController> logger, 
            IAudioPlatformService audioPlatformService,
            IAzureStorageServiceFactory azureStorageServiceFactory,
            IPollyRetryService pollyRetryService,
            IBackgroundWorkerQueue backgroundWorkerQueue,
            IFeatureToggles featureToggles)
        {
            _queryHandler = queryHandler;
            _commandHandler = commandHandler;
            _videoPlatformService = videoPlatformService;
            _supplierConfiguration = supplierLocator.GetSupplierConfiguration();
            _logger = logger;
            _audioPlatformService = audioPlatformService;
            _azureStorageServiceFactory = azureStorageServiceFactory;
            _pollyRetryService = pollyRetryService;
            _backgroundWorkerQueue = backgroundWorkerQueue;
            _featureToggles = featureToggles;
        }

        /// <summary>
        /// Request to book a conference
        /// </summary>
        /// <param name="request">Details of a conference</param>
        /// <returns>Details of the new conference</returns>
        [HttpPost]
        [OpenApiOperation("BookNewConference")]
        [ProducesResponseType(typeof(ConferenceDetailsResponse), (int) HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails),(int)HttpStatusCode.BadRequest)]
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

            var audioIngestUrl = _featureToggles.HrsIntegrationEnabled() ? 
                _audioPlatformService.GetAudioIngestUrl(request.CaseTypeServiceId, request.CaseNumber, request.HearingRefId.ToString()) 
                : _audioPlatformService.GetAudioIngestUrl(request.HearingRefId.ToString());
      
            var conferenceId = await CreateConferenceWithRetiesAsync(request, audioIngestUrl);
            _logger.LogDebug("Conference Created");

            var conferenceEndpoints =
                await _queryHandler.Handle<GetEndpointsForConferenceQuery, IList<Endpoint>>(
                    new GetEndpointsForConferenceQuery(conferenceId));
            var endpointDtos = conferenceEndpoints.Select(EndpointMapper.MapToEndpoint);

            var roomBookedSuccess = await BookMeetingRoomWithRetriesAsync(conferenceId, request.AudioRecordingRequired, audioIngestUrl, endpointDtos);
            
            if (!roomBookedSuccess)
            {
                var message = $"Could not book and find meeting room for conferenceId: {conferenceId}";
                _logger.LogError(message);
                return StatusCode((int) HttpStatusCode.InternalServerError, message);
            }
            
            _logger.LogDebug("Room Booked");

            var getConferenceByIdQuery = new GetConferenceByIdQuery(conferenceId);
            var queriedConference = await _queryHandler.Handle<GetConferenceByIdQuery, Conference>(getConferenceByIdQuery);

            var response = ConferenceToDetailsResponseMapper.MapConferenceToResponse(queriedConference, _supplierConfiguration);

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
        [ProducesResponseType(typeof(ValidationProblemDetails),(int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateConferenceAsync(UpdateConferenceRequest request)
        {
            _logger.LogDebug("UpdateConference");

            var query = new GetNonClosedConferenceByHearingRefIdQuery(request.HearingRefId);
            var conferencesList = await _queryHandler.Handle<GetNonClosedConferenceByHearingRefIdQuery, List<Conference>>(query);
            var conference = conferencesList.FirstOrDefault();
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
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails),(int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetConferenceDetailsByIdAsync(Guid conferenceId)
        {
            _logger.LogDebug("GetConferenceDetailsById {ConferenceId}", conferenceId);

            var getConferenceByIdQuery = new GetConferenceByIdQuery(conferenceId);
            var queriedConference =
                await _queryHandler.Handle<GetConferenceByIdQuery, Conference>(getConferenceByIdQuery);

            if (queriedConference == null)
            {
                _logger.LogWarning("Unable to find conference {ConferenceId}", conferenceId);

                return NotFound($"Unable to find a conference with the given id {conferenceId}");
            }

            var response =
                ConferenceToDetailsResponseMapper.MapConferenceToResponse(queriedConference, _supplierConfiguration);
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
        [ProducesResponseType(typeof(ValidationProblemDetails),(int)HttpStatusCode.BadRequest)]
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
        /// Get conferences by hearing ref id
        /// </summary>
        /// <param name="hearingRefId">Hearing ID</param>
        /// <param name="includeClosed">Include closed conferences in search</param>
        /// <returns>Full details including participants and statuses of a conference</returns>
        [HttpGet("hearings/{hearingRefId}")]
        [OpenApiOperation("GetConferenceByHearingRefId")]
        [ProducesResponseType(typeof(ConferenceDetailsResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails),(int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetConferenceByHearingRefIdAsync(Guid hearingRefId, [FromQuery]bool? includeClosed = false)
        {
            _logger.LogDebug("GetConferenceByHearingRefId {HearingRefId}", hearingRefId);

            var query = new GetNonClosedConferenceByHearingRefIdQuery(hearingRefId, includeClosed.GetValueOrDefault());

            var conferencesList = await _queryHandler.Handle<GetNonClosedConferenceByHearingRefIdQuery, List<Conference>>(query);
            var conference = conferencesList.FirstOrDefault();
            
            if (conference == null)
            {
                _logger.LogWarning("Unable to find conference with hearing id {HearingRefId}", hearingRefId);

                return NotFound();
            }

            var response = ConferenceToDetailsResponseMapper.MapConferenceToResponse(conference, _supplierConfiguration);

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

        public async Task<IActionResult> GetExpiredAudioRecordingConferencesAsync()
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
        [ProducesResponseType(typeof(ValidationProblemDetails),(int)HttpStatusCode.BadRequest)]
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
        [ProducesResponseType(typeof(List<ParticipantInHearingResponse>), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> GetJudgesInHearingsTodayAsync()
        {
            _logger.LogDebug("GetJudgesInHearingsToday");
            var conferences = await _queryHandler.Handle<GetHostsInHearingsTodayQuery, List<Conference>>(new GetHostsInHearingsTodayQuery(true));
            var response = conferences.Select(ParticipantInHearingResponseMapper.MapConferenceSummaryToJudgeInHearingResponse);
            return Ok(response);
        }

        /// <summary>
        /// Get today's conferences where hosts are in hearings
        /// </summary>
        /// <returns>Conference details</returns>
        [HttpGet("today/hostsinhearings")]
        [OpenApiOperation("GetHostsInHearingsToday")]
        [ProducesResponseType(typeof(List<ParticipantInHearingResponse>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetHostsInHearingsTodayAsync()
        {
            _logger.LogDebug("GetHostsInHearingsToday");
            var conferences = await _queryHandler.Handle<GetHostsInHearingsTodayQuery, List<Conference>>(new GetHostsInHearingsTodayQuery());
            var response = conferences.Select(ParticipantInHearingResponseMapper.MapConferenceSummaryToHostInHearingResponse);
            return Ok(response);
        }
        
        /// <summary>
        /// Get conferences Hearing rooms
        /// </summary>
        /// <returns>Hearing rooms details</returns>
        [HttpGet("hearingRooms")]
        [OpenApiOperation("GetConferencesHearingRooms")]
        [ProducesResponseType(typeof(List<ConferenceHearingRoomsResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.NoContent)]
        public async Task<IActionResult> GetConferencesHearingRoomsAsync([FromQuery]string date)
        {
            _logger.LogDebug("GetConferencesHearingRooms");

            try
            {
                var requestedDate = DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                var interpreterRooms = await _queryHandler.Handle<GetConferenceInterpreterRoomsByDateQuery, List<HearingAudioRoom>>(
                        new GetConferenceInterpreterRoomsByDateQuery(requestedDate));

                var conferences =
                    await _queryHandler.Handle<GetConferenceHearingRoomsByDateQuery, List<HearingAudioRoom>>(
                        new GetConferenceHearingRoomsByDateQuery(requestedDate));


                conferences.AddRange(interpreterRooms);

                var response = ConferenceHearingRoomsResponseMapper.Map(conferences, requestedDate);

                return Ok(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return NoContent();
            }
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
            await _backgroundWorkerQueue.QueueBackgroundWorkItem(removeHeartbeatsCommand);

            _logger.LogInformation($"Successfully removed heartbeats for conferences");
            return NoContent();
        }

        /// <summary>
        /// Anonymise conference with matching hearing ids
        /// </summary>
        /// <param name="request">hearing ids of expired conferences</param>
        /// <returns></returns>
        [HttpPatch("anonymise-conference-with-hearing-ids")]
        [OpenApiOperation("AnonymiseConferenceWithHearingIds")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        public async Task<IActionResult> AnonymiseConferenceWithHearingIds(
            AnonymiseConferenceWithHearingIdsRequest request)
        {
            await _commandHandler.Handle(new AnonymiseConferenceWithHearingIdsCommand
                { HearingIds = request.HearingIds });
            return Ok();
        }

        private async Task SafelyRemoveCourtRoomAsync(Guid conferenceId)
        {
            var meetingRoom = await _videoPlatformService.GetVirtualCourtRoomAsync(conferenceId);
            if (meetingRoom != null)
            {
                await _videoPlatformService.DeleteVirtualCourtRoomAsync(conferenceId);
            }
        }

        [HttpGet("Wowza/ReconcileAudioFilesInStorage")]
        [OpenApiOperation("ReconcileAudioFilesInStorage")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> ReconcileAudioFilesInStorage([FromQuery] AudioFilesInStorageRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.FileNamePrefix))
            {
                var msg = $"ReconcileFilesInStorage - File Name prefix is required.";
                throw new AudioPlatformFileNotFoundException(msg, HttpStatusCode.NotFound);
            }

            if (request.FilesCount <= 0)
            {
                var msg = $"ReconcileFilesInStorage - File count cannot be negative or zero.";
                throw new AudioPlatformFileNotFoundException(msg, HttpStatusCode.NotFound);
            }

            try
            {
                var azureStorageService = _azureStorageServiceFactory.Create(AzureStorageServiceType.Vh);

                var result = await azureStorageService.ReconcileFilesInStorage(request.FileNamePrefix, request.FilesCount);

                return Ok(result);
            }
            catch (Exception e)
            {
                throw new AudioPlatformFileNotFoundException(e.Message, HttpStatusCode.InternalServerError);
            }
            
        }

        public async Task<bool> BookMeetingRoomAsync(Guid conferenceId,
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
                _logger.LogError(ex, "Room already booked for conference {conferenceId}", conferenceId);

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
                new CheckConferenceOpenQuery(request.ScheduledDateTime, request.CaseNumber, request.CaseName, request.HearingRefId));

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

        private async Task<bool> BookMeetingRoomWithRetriesAsync(Guid conferenceId,
            bool audioRecordingRequired,
            string ingestUrl,
            IEnumerable<EndpointDto> endpoints)
        {
            var result = await _pollyRetryService.WaitAndRetryAsync<Exception, bool>
            (
                3,
                _ => TimeSpan.FromSeconds(10),
                retryAttempt => _logger.LogWarning($"Failed to BookMeetingRoomAsync. Retrying attempt {retryAttempt}"),
                callResult => !callResult,
                () => BookMeetingRoomAsync(conferenceId, audioRecordingRequired, ingestUrl, endpoints)
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
     
        private async Task DeleteAudioRecordingApplication(Guid conferenceId)
        {
            var getConferenceByIdQuery = new GetConferenceByIdQuery(conferenceId);
            var queriedConference =
                await _queryHandler.Handle<GetConferenceByIdQuery, Conference>(getConferenceByIdQuery);
            
            if (queriedConference is {AudioRecordingRequired: true} &&
                !queriedConference.IngestUrl.Contains(_audioPlatformService.ApplicationName)) //should not delete application, if on single instance version
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
    }
}
