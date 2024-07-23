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
using VideoApi.Validations;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.Controllers
{
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("conferences")]
    [ApiController]
    [SuppressMessage("Info Code Smell", "S1133:Deprecated code should be removed")]
    public class ConferenceController(
        IQueryHandler queryHandler,
        ICommandHandler commandHandler,
        IVideoPlatformService videoPlatformService,
        ISupplierApiSelector supplierLocator,
        ILogger<ConferenceController> logger,
        IAudioPlatformService audioPlatformService,
        IAzureStorageServiceFactory azureStorageServiceFactory,
        IPollyRetryService pollyRetryService,
        IBackgroundWorkerQueue backgroundWorkerQueue)
        : ControllerBase
    {
        private readonly SupplierConfiguration _supplierConfiguration = supplierLocator.GetSupplierConfiguration();
        
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
            logger.LogDebug("BookNewConference");

            foreach (var participant in request.Participants)
            {
                participant.Username = participant.Username.ToLower().Trim();
                participant.Name = participant.Name.Trim();
                participant.FirstName = participant.FirstName.Trim();
                participant.LastName = participant.LastName.Trim();
                participant.DisplayName = participant.DisplayName.Trim();
            }

            var audioIngestUrl = audioPlatformService.GetAudioIngestUrl(request.CaseTypeServiceId, request.CaseNumber, request.HearingRefId.ToString());
      
            var conferenceId = await CreateConferenceWithRetiesAsync(request, audioIngestUrl);
            logger.LogDebug("Conference Created");

            var conferenceEndpoints =
                await queryHandler.Handle<GetEndpointsForConferenceQuery, IList<Endpoint>>(
                    new GetEndpointsForConferenceQuery(conferenceId));
            var endpointDtos = conferenceEndpoints.Select(EndpointMapper.MapToEndpoint);

            var roomBookedSuccess = await BookMeetingRoomWithRetriesAsync(conferenceId, request.AudioRecordingRequired, audioIngestUrl, endpointDtos);
            
            if (!roomBookedSuccess)
            {
                var message = $"Could not book and find meeting room for conferenceId: {conferenceId}";
                logger.LogError(message);
                return StatusCode((int) HttpStatusCode.InternalServerError, message);
            }
            
            logger.LogDebug("Room Booked");

            var getConferenceByIdQuery = new GetConferenceByIdQuery(conferenceId);
            var queriedConference = await queryHandler.Handle<GetConferenceByIdQuery, Conference>(getConferenceByIdQuery);

            var response = ConferenceToDetailsResponseMapper.MapConferenceToResponse(queriedConference, _supplierConfiguration.PexipSelfTestNode);

            logger.LogInformation("Created conference {ResponseId} for hearing {HearingRefId}", response.Id, request.HearingRefId);

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
            logger.LogDebug("UpdateConference");

            var query = new GetNonClosedConferenceByHearingRefIdQuery(request.HearingRefId);
            var conferencesList = await queryHandler.Handle<GetNonClosedConferenceByHearingRefIdQuery, List<Conference>>(query);
            var conference = conferencesList.FirstOrDefault();
            if (conference == null)
            {
                logger.LogWarning("Unable to find conference with hearing id {HearingRefId}", request.HearingRefId);

                return NotFound();
            }

            var endpointDtos = conference.GetEndpoints().Select(EndpointMapper.MapToEndpoint);
            await videoPlatformService.UpdateVirtualCourtRoomAsync(conference.Id, request.AudioRecordingRequired,
                endpointDtos);

            try
            {
                var command = new UpdateConferenceDetailsCommand(request.HearingRefId, request.CaseNumber,
                    request.CaseType, request.CaseName, request.ScheduledDuration, request.ScheduledDateTime,
                    request.HearingVenueName, request.AudioRecordingRequired);

                await commandHandler.Handle(command);

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
            logger.LogDebug("GetConferenceDetailsById {ConferenceId}", conferenceId);

            var getConferenceByIdQuery = new GetConferenceByIdQuery(conferenceId);
            var queriedConference =
                await queryHandler.Handle<GetConferenceByIdQuery, Conference>(getConferenceByIdQuery);

            if (queriedConference == null)
            {
                logger.LogWarning("Unable to find conference {ConferenceId}", conferenceId);

                return NotFound($"Unable to find a conference with the given id {conferenceId}");
            }

            var response =
                ConferenceToDetailsResponseMapper.MapConferenceToResponse(queriedConference, _supplierConfiguration.PexipSelfTestNode);
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
            logger.LogDebug("RemoveConference {ConferenceId}", conferenceId);
            var removeConferenceCommand = new RemoveConferenceCommand(conferenceId);
            try
            {
                await commandHandler.Handle(removeConferenceCommand);
                await SafelyRemoveCourtRoomAsync(conferenceId);

                logger.LogInformation("Successfully removed conference {ConferenceId}", conferenceId);

                return NoContent();
            }
            catch (ConferenceNotFoundException ex)
            {
                logger.LogError(ex, "Unable to find conference {ConferenceId}", conferenceId);

                return NotFound();
            }
        }

        /// <summary>
        /// Get today's conferences by HearingVenueName
        /// </summary>
        /// <returns>Conference details</returns>
        [HttpGet("today/vho")]
        [OpenApiOperation("GetConferencesTodayForAdminByHearingVenueName")]
        [ProducesResponseType(typeof(List<ConferenceForAdminResponse>), (int)HttpStatusCode.OK)]
        [Obsolete("Use booking-api:GetHearingsForTodayByVenue instead", false)]
        public async Task<IActionResult> GetConferencesTodayForAdminByHearingVenueNameAsync([FromQuery] ConferenceForAdminRequest request)
        {
            logger.LogDebug("GetConferencesTodayForAdmin");

            var query = new GetConferencesTodayForAdminByHearingVenueNameQuery
            {
                HearingVenueNames = request.HearingVenueNames
            };

            var conferences = await queryHandler.Handle<GetConferencesTodayForAdminByHearingVenueNameQuery, List<Conference>>(query);
            var response = conferences.Select(c => ConferenceForAdminResponseMapper.MapConferenceToAdminResponse(c, _supplierConfiguration));

            return Ok(response);
        }

        /// <summary>
        /// Get today's conferences by HearingVenueName for staff members
        /// </summary>
        /// <returns>Conference details</returns>
        [HttpGet("today/staff-member")]
        [OpenApiOperation("GetConferencesTodayForStaffMemberByHearingVenueName")]
        [ProducesResponseType(typeof(List<ConferenceForHostResponse>), (int)HttpStatusCode.OK)]
        [Obsolete("Use booking-api:GetHearingsForTodayByVenue instead", false)]
        public async Task<IActionResult> GetConferencesTodayForStaffMemberByHearingVenueName([FromQuery] ConferenceForStaffMembertWithSelectedVenueRequest request)
        {
            logger.LogDebug("GetConferencesTodayForAdmin");

            var query = new GetConferencesTodayForStaffMemberByHearingVenueNameQuery
            {
                HearingVenueNames = request.HearingVenueNames
            };

            var conferences = await queryHandler.Handle<GetConferencesTodayForStaffMemberByHearingVenueNameQuery, List<Conference>>(query);

            return Ok(conferences.Select(ConferenceForHostResponseMapper.MapConferenceSummaryToModel));
        }

        /// <summary>
        /// Get all conferences for a judge
        /// </summary>
        /// <param name="username">judge username</param>
        /// <returns>List of conferences for judge</returns>
        [HttpGet("today/judge")]
        [OpenApiOperation("GetConferencesTodayForJudgeByUsername")]
        [ProducesResponseType(typeof(List<ConferenceForHostResponse>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails),(int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetConferencesTodayForJudgeByUsernameAsync([FromQuery] string username)
        {
            logger.LogDebug("GetConferencesTodayForJudgeByUsername {Username}", username);

            var response = await GetHostConferencesForToday(username);

            if (response is null)
            {
                return ValidationProblem(ModelState);
            }

            return Ok(response);
        }

        /// <summary>
        /// Get all conferences for a host
        /// </summary>
        /// <param name="username">Host username</param>
        /// <returns>List of conferences for host</returns>
        [HttpGet("today/host")]
        [OpenApiOperation("GetConferencesTodayForHost")]
        [ProducesResponseType(typeof(List<ConferenceForHostResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetConferencesTodayForHostAsync([FromQuery] string username)
        {
            logger.LogDebug("GetConferencesTodayForHost {Username}", username);

            var response = await GetHostConferencesForToday(username);

            if (response is null)
            {
                return BadRequest(ModelState);
            }

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
        [ProducesResponseType(typeof(ValidationProblemDetails),(int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetConferencesTodayForIndividualByUsernameAsync([FromQuery] string username)
        {
            logger.LogDebug("GetConferencesTodayForIndividualByUsername {Username}", username);

            if (!username.IsValidEmail())
            {
                ModelState.AddModelError(nameof(username), $"Please provide a valid {nameof(username)}");

                logger.LogWarning("Invalid username {Username}", username);

                return ValidationProblem(ModelState);
            }

            var query = new GetConferencesForTodayByIndividualQuery(username.ToLower().Trim());
            var conferences =
                await queryHandler.Handle<GetConferencesForTodayByIndividualQuery, List<Conference>>(query);
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
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails),(int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetConferenceByHearingRefIdAsync(Guid hearingRefId, [FromQuery]bool? includeClosed = false)
        {
            logger.LogDebug("GetConferenceByHearingRefId {HearingRefId}", hearingRefId);

            var query = new GetNonClosedConferenceByHearingRefIdQuery(hearingRefId, includeClosed.GetValueOrDefault());

            var conferencesList = await queryHandler.Handle<GetNonClosedConferenceByHearingRefIdQuery, List<Conference>>(query);
            var conference = conferencesList.FirstOrDefault();
            
            if (conference == null)
            {
                logger.LogWarning("Unable to find conference with hearing id {HearingRefId}", hearingRefId);

                return NotFound();
            }

            var response = ConferenceToDetailsResponseMapper.MapConferenceToResponse(conference, _supplierConfiguration.PexipSelfTestNode);

            return Ok(response);
        }

        /// <summary>
        /// Get conferences by hearing ref id
        /// </summary>
        /// <param name="request">Hearing ref IDs</param>
        /// <returns>Full details including participants and statuses of a conference</returns>
        [HttpPost("hearings/staff-member")]
        [OpenApiOperation("GetConferencesForAdminByHearingRefId")]
        [ProducesResponseType(typeof(List<ConferenceForAdminResponse>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails),(int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetConferencesForAdminByHearingRefIdAsync(GetConferencesByHearingIdsRequest request)
        {
            var query = new GetNonClosedConferenceByHearingRefIdQuery(request.HearingRefIds, true);
            var conferences = await queryHandler.Handle<GetNonClosedConferenceByHearingRefIdQuery, List<Conference>>(query);

            if (conferences.Count <= 0)
                return NotFound();

            var response = conferences
                .Select(conference =>  ConferenceForAdminResponseMapper.MapConferenceToAdminResponse(conference, _supplierConfiguration))
                .ToList();

            return Ok(response);
        }
        
        /// <summary>
        /// Get conferences by hearing ref id
        /// </summary>
        /// <param name="request">Hearing ref IDs</param>
        /// <returns>Full details including participants and statuses of a conference</returns>
        [HttpPost("hearings/host")]
        [OpenApiOperation("GetConferencesForHostByHearingRefId")]
        [ProducesResponseType(typeof(List<ConferenceForHostResponse>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails),(int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetConferencesForHostByHearingRefIdAsync(GetConferencesByHearingIdsRequest request)
        {
            var query = new GetNonClosedConferenceByHearingRefIdQuery(request.HearingRefIds, true);
            var conferences = await queryHandler.Handle<GetNonClosedConferenceByHearingRefIdQuery, List<Conference>>(query);

            if (conferences.Count <= 0)
                return NotFound();

            return Ok(conferences.Select(ConferenceForHostResponseMapper.MapConferenceSummaryToModel).ToList());
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
            logger.LogDebug("GetExpiredOpenConferences");

            var query = new GetExpiredUnclosedConferencesQuery();
            var conferences = await queryHandler.Handle<GetExpiredUnclosedConferencesQuery, List<Conference>>(query);
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
            logger.LogDebug("GetExpiredAudiorecordingConferences");
            var query = new GetExpiredAudiorecordingConferencesQuery();
            var conferences =
                await queryHandler.Handle<GetExpiredAudiorecordingConferencesQuery, List<Conference>>(query);
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

                await commandHandler.Handle(command);
                await SafelyRemoveCourtRoomAsync(conferenceId);
                await DeleteAudioRecordingApplication(conferenceId);

                return NoContent();
            }
            catch (ConferenceNotFoundException ex)
            {
                logger.LogError(ex, "Unable to find conference {conferenceId}", conferenceId);

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
            logger.LogDebug("GetJudgesInHearingsToday");

            var response = await GetHostsInHearingsToday(judgesOnly: true);

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
            logger.LogDebug("GetHostsInHearingsToday");

            var response = await GetHostsInHearingsToday();

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
            logger.LogDebug("GetConferencesHearingRooms");

            try
            {
                var requestedDate = DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                var interpreterRooms = await queryHandler.Handle<GetConferenceInterpreterRoomsByDateQuery, List<HearingAudioRoom>>(
                        new GetConferenceInterpreterRoomsByDateQuery(requestedDate));

                var conferences =
                    await queryHandler.Handle<GetConferenceHearingRoomsByDateQuery, List<HearingAudioRoom>>(
                        new GetConferenceHearingRoomsByDateQuery(requestedDate));


                conferences.AddRange(interpreterRooms);

                var response = ConferenceHearingRoomsResponseMapper.Map(conferences, requestedDate);

                return Ok(response);
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
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
            logger.LogDebug("AnonymiseConferencesAndParticipantInformation");

            var anonymiseConferenceCommand = new AnonymiseConferencesCommand();
            await commandHandler.Handle(anonymiseConferenceCommand);

            logger.LogInformation("Records updated: {RecordsUpdated}", anonymiseConferenceCommand.RecordsUpdated);
            return NoContent();
        }

        [HttpDelete("expiredHearbeats")]
        [OpenApiOperation("RemoveHeartbeatsForConferences")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public async Task<IActionResult> RemoveHeartbeatsForConferencesAsync()
        {
            logger.LogDebug("Remove heartbeats for conferences over 14 days old.");

            var removeHeartbeatsCommand = new RemoveHeartbeatsForConferencesCommand();
            await backgroundWorkerQueue.QueueBackgroundWorkItem(removeHeartbeatsCommand);

            logger.LogInformation($"Successfully removed heartbeats for conferences");
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
            await commandHandler.Handle(new AnonymiseConferenceWithHearingIdsCommand
                { HearingIds = request.HearingIds });
            return Ok();
        }

        private async Task SafelyRemoveCourtRoomAsync(Guid conferenceId)
        {
            var meetingRoom = await videoPlatformService.GetVirtualCourtRoomAsync(conferenceId);
            if (meetingRoom != null)
            {
                await videoPlatformService.DeleteVirtualCourtRoomAsync(conferenceId);
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
                var azureStorageService = azureStorageServiceFactory.Create(AzureStorageServiceType.Vh);

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
                meetingRoom = await videoPlatformService.BookVirtualCourtroomAsync(conferenceId,
                    audioRecordingRequired,
                    ingestUrl,
                    endpoints);
            }
            catch (DoubleBookingException ex)
            {
                logger.LogError(ex, "Room already booked for conference {conferenceId}", conferenceId);

                meetingRoom = await videoPlatformService.GetVirtualCourtRoomAsync(conferenceId);
            }

            if (meetingRoom == null)  return false;

            var command = new UpdateMeetingRoomCommand
            (
                conferenceId, meetingRoom.AdminUri, meetingRoom.JudgeUri, meetingRoom.ParticipantUri,
                meetingRoom.PexipNode, meetingRoom.TelephoneConferenceId
            );

            await commandHandler.Handle(command);

            return true;
        }
        
        private async Task<Guid> CreateConferenceAsync(BookNewConferenceRequest request, string ingestUrl)
        {
            var existingConference = await queryHandler.Handle<CheckConferenceOpenQuery, Conference>(
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

            await commandHandler.Handle(createConferenceCommand);

            return createConferenceCommand.NewConferenceId;
        }

        private async Task<bool> BookMeetingRoomWithRetriesAsync(Guid conferenceId,
            bool audioRecordingRequired,
            string ingestUrl,
            IEnumerable<EndpointDto> endpoints)
        {
            var result = await pollyRetryService.WaitAndRetryAsync<Exception, bool>
            (
                3,
                _ => TimeSpan.FromSeconds(10),
                retryAttempt => logger.LogWarning($"Failed to BookMeetingRoomAsync. Retrying attempt {retryAttempt}"),
                callResult => !callResult,
                () => BookMeetingRoomAsync(conferenceId, audioRecordingRequired, ingestUrl, endpoints)
            );

            return result;
        }

        private async Task<Guid> CreateConferenceWithRetiesAsync(BookNewConferenceRequest request, string ingestUrl)
        {
            var result = await pollyRetryService.WaitAndRetryAsync<Exception, Guid>
            (
                3,
                _ => TimeSpan.FromSeconds(10),
                retryAttempt => logger.LogWarning("Failed to CreateConferenceAsync. Retrying attempt {RetryAttempt}", retryAttempt),
                callResult => callResult == Guid.Empty,
                async () => await CreateConferenceAsync(request, ingestUrl));

            return result;
        }

        private async Task<IEnumerable<ConferenceForHostResponse>> GetHostConferencesForToday(string username)
        {

            if (!username.IsValidEmail())
            {
                ModelState.AddModelError(nameof(username), $"Please provide a valid {nameof(username)}");

                logger.LogWarning("Invalid username {Username}", username);

                return null;
            }

            var query = new GetConferencesForTodayByHostQuery(username.ToLower().Trim());
            var conferences = await queryHandler.Handle<GetConferencesForTodayByHostQuery, List<Conference>>(query);
            var conferenceForHostResponse = conferences.Select(ConferenceForHostResponseMapper.MapConferenceSummaryToModel);
            return conferenceForHostResponse;
        }
     
        private async Task DeleteAudioRecordingApplication(Guid conferenceId)
        {
            var getConferenceByIdQuery = new GetConferenceByIdQuery(conferenceId);
            var queriedConference =
                await queryHandler.Handle<GetConferenceByIdQuery, Conference>(getConferenceByIdQuery);
            
            if (queriedConference is {AudioRecordingRequired: true} &&
                !queriedConference.IngestUrl.Contains(audioPlatformService.ApplicationName)) //should not delete application, if on single instance version
            {
                try
                {
                    await EnsureAudioFileExists(queriedConference);
                    await audioPlatformService.DeleteAudioApplicationAsync(queriedConference.HearingRefId);
                }
                catch (AudioPlatformFileNotFoundException ex)
                {
                    logger.LogError(ex, ex.Message);
                }

            }
        }

        private async Task EnsureAudioFileExists(Conference conference)
        {
            var azureStorageService = azureStorageServiceFactory.Create(AzureStorageServiceType.Vh);
            var allBlobs = await azureStorageService.GetAllBlobNamesByFilePathPrefix(conference.HearingRefId.ToString());

            if (!allBlobs.Any() && conference.ActualStartTime.HasValue)
            {
                var msg = $"Audio recording file not found for hearing: {conference.HearingRefId}";
                throw new AudioPlatformFileNotFoundException(msg, HttpStatusCode.NotFound);
            }
        }
        private async Task<IEnumerable<ParticipantInHearingResponse>> GetHostsInHearingsToday(bool judgesOnly = false)
        {
            var conferences =
                await queryHandler.Handle<GetHostsInHearingsTodayQuery, List<Conference>>(
                    new GetHostsInHearingsTodayQuery(judgesOnly));

            return judgesOnly
                ? conferences.SelectMany(ConferenceForHostResponseMapper.MapConferenceSummaryToJudgeInHearingResponse)
                : conferences.SelectMany(ConferenceForHostResponseMapper.MapConferenceSummaryToHostInHearingResponse);
        }
    }
}
