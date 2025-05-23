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
using VideoApi.DAL.DTOs;
using VideoApi.DAL.Exceptions;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Extensions;
using VideoApi.Mappings;
using VideoApi.Services;
using VideoApi.Common.Logging;

namespace VideoApi.Controllers
{
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("conferences")]
    [ApiController]
    public class ParticipantsController : ControllerBase
    {
        private readonly ICommandHandler _commandHandler;
        private readonly ILogger<ParticipantsController> _logger;
        private readonly IQueryHandler _queryHandler;
        private readonly ISupplierPlatformServiceFactory _supplierPlatformServiceFactory;

        public ParticipantsController(ICommandHandler commandHandler, IQueryHandler queryHandler,
            ISupplierPlatformServiceFactory supplierPlatformServiceFactory, ILogger<ParticipantsController> logger)
        {
            _commandHandler = commandHandler;
            _queryHandler = queryHandler;
            _supplierPlatformServiceFactory = supplierPlatformServiceFactory;
            _logger = logger;
        }
        
        /// <summary>
        /// Add participants to a conference
        /// </summary>
        /// <param name="conferenceId">The id of the conference to add participants to</param>
        /// <param name="request">Details of the participant</param>
        /// <returns></returns>
        [HttpPut("{conferenceId}/participants")]
        [OpenApiOperation("AddParticipantsToConference")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddParticipantsToConferenceAsync(Guid conferenceId,
            AddParticipantsToConferenceRequest request)
        {
            _logger.LogAddParticipantsToConference();
            var participants = request.Participants.Select(x =>
                    new Participant(x.ParticipantRefId,
                        x.DisplayName.Trim(), x.Username.ToLowerInvariant().Trim(), x.UserRole.MapToDomainEnum(),
                        x.HearingRole, x.ContactEmail))
                .ToList();
            
            var linkedParticipants = request.Participants
                .SelectMany(x => x.LinkedParticipants)
                .Select(x => new LinkedParticipantDto()
                {
                    ParticipantRefId = x.ParticipantRefId,
                    LinkedRefId = x.LinkedRefId,
                    Type = x.Type.MapToDomainEnum()
                }).ToList();
            
            try
            {
                var addParticipantCommand = new AddParticipantsToConferenceCommand(conferenceId,
                    participants.Select(x => x as ParticipantBase).ToList(), linkedParticipants);
                
                await _commandHandler.Handle(addParticipantCommand);
                
                return NoContent();
            }
            catch (ConferenceNotFoundException ex)
            {
                _logger.LogUnableToFindConferenceError(ex);
                return NotFound();
            }
        }
        
        
        /// <summary>
        /// Update a conference's participants
        /// </summary>
        /// <param name="conferenceId">Id of conference to look up</param>
        /// <param name="request">Information about the participants</param>
        /// <returns></returns>
        [HttpPatch("{conferenceId}/UpdateConferenceParticipants", Name = "UpdateConferenceParticipants")]
        [OpenApiOperation("UpdateConferenceParticipants")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateConferenceParticipantsAsync(Guid conferenceId,
            UpdateConferenceParticipantsRequest request)
        {
            _logger.LogUpdateConferenceParticipants();
            try
            {
                var existingParticipants = request.ExistingParticipants.Select(x =>
                        new Participant(x.ParticipantRefId, x.ContactEmail, x.DisplayName,
                            x.Username))
                    .ToList();
                
                var newParticipants = request.NewParticipants.Select(x =>
                        new Participant(x.ParticipantRefId,
                            x.DisplayName.Trim(), x.Username.ToLowerInvariant().Trim(), x.UserRole.MapToDomainEnum(),
                            x.HearingRole, x.ContactEmail, x.Id))
                    .ToList();
                
                var linkedParticipants = request.LinkedParticipants
                    .Select(x => new LinkedParticipantDto()
                    {
                        ParticipantRefId = x.ParticipantRefId,
                        LinkedRefId = x.LinkedRefId,
                        Type = x.Type.MapToDomainEnum()
                    }).ToList();
                
                var updateHearingParticipantsCommand = new UpdateConferenceParticipantsCommand(conferenceId,
                    existingParticipants.Select(x => x as ParticipantBase).ToList(),
                    newParticipants.Select(x => x as ParticipantBase).ToList(),
                    request.RemovedParticipants, linkedParticipants);
                
                await _commandHandler.Handle(updateHearingParticipantsCommand);
                
                return NoContent();
            }
            catch (ConferenceNotFoundException ex)
            {
                _logger.LogUnableToFindConferenceError(ex);
                return NotFound($"Unable to find conference {conferenceId}");
            }
            catch (ParticipantNotFoundException ex)
            {
                _logger.LogUnableToFindParticipantError(ex);
                return NotFound($"Unable to find participant {ex.ParticipantId} in conference {conferenceId}");
            }
        }
        
        /// <summary>
        /// Update participant details
        /// </summary>
        /// <param name="conferenceId">Id of conference to look up</param>
        /// <param name="participantId">Id of participant to remove</param>
        /// <param name="request">The participant information to update</param>
        /// <returns></returns>
        [HttpPatch("{conferenceId}/participants/{participantId}", Name = "UpdateParticipantDetails")]
        [OpenApiOperation("UpdateParticipantDetails")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateParticipantDetailsAsync(Guid conferenceId, Guid participantId, UpdateParticipantRequest request)
        {
            _logger.LogUpdateParticipantDetails();
            try
            {
                var conference = await _queryHandler.Handle<GetConferenceByIdQuery, Conference>(new GetConferenceByIdQuery(conferenceId));
                
                if(conference == null)
                    throw new ConferenceNotFoundException(conferenceId);
                
                var linkedParticipants = request.LinkedParticipants.Select(x => new LinkedParticipantDto()
                {
                    ParticipantRefId = x.ParticipantRefId,
                    LinkedRefId = x.LinkedRefId,
                    Type = x.Type.MapToDomainEnum()
                }).ToList();
                
                var updateParticipantDetailsCommand = new UpdateParticipantDetailsCommand(conferenceId,
                    participantId,
                    request.DisplayName,
                    request.ContactEmail,
                    linkedParticipants,
                    request.UserRole.MapToDomainEnum(),
                    request.HearingRole);
                
                if (!string.IsNullOrEmpty(request.Username))
                {
                    updateParticipantDetailsCommand.Username = request.Username;
                }
                await _commandHandler.Handle(updateParticipantDetailsCommand);
                var videoPlatformService = _supplierPlatformServiceFactory.Create(conference.Supplier);
                await videoPlatformService.UpdateParticipantName(conferenceId, participantId, request.DisplayName);
                
                return NoContent();
            }
            catch (ConferenceNotFoundException ex)
            {
                _logger.LogUnableToFindConferenceError(ex);
                return NotFound();
            }
            catch (ParticipantNotFoundException ex)
            {
                _logger.LogUnableToFindParticipantError(ex);
                return NotFound();
            }
        }
        
        /// <summary>
        /// Remove participants from a conference
        /// </summary>
        /// <param name="conferenceId">The id of the conference to remove participants from</param>
        /// <param name="participantId">The id of the participant to remove</param>
        /// <returns></returns>
        [HttpDelete("{conferenceId}/participants/{participantId}")]
        [OpenApiOperation("RemoveParticipantFromConference")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> RemoveParticipantFromConferenceAsync(Guid conferenceId, Guid participantId)
        {
            _logger.LogRemoveParticipantFromConference();
            var getConferenceByIdQuery = new GetConferenceByIdQuery(conferenceId);
            var queriedConference =
                await _queryHandler.Handle<GetConferenceByIdQuery, Conference>(getConferenceByIdQuery);
            
            if (queriedConference == null)
            {
                _logger.LogUnableToFindConference();
                return NotFound();
            }
            
            var participant = queriedConference.GetParticipants().SingleOrDefault(x => x.Id == participantId);
            if (participant == null)
            {
                _logger.LogUnableToFindParticipant(participantId);
                return NotFound();
            }
            
            var participants = new List<ParticipantBase> { participant };
            var command = new RemoveParticipantsFromConferenceCommand(conferenceId, participants);
            await _commandHandler.Handle(command);
            return NoContent();
        }
        
        /// <summary>
        /// Get the test call result for a participant
        /// </summary>
        /// <param name="conferenceId">The id of the conference</param>
        /// <param name="participantId">The id of the participant</param>
        /// <returns></returns>
        [HttpGet("{conferenceId}/participants/{participantId}/selftestresult")]
        [OpenApiOperation("GetTestCallResultForParticipant")]
        [ProducesResponseType(typeof(TestCallScoreResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetTestCallResultForParticipantAsync(Guid conferenceId, Guid participantId)
        {
            _logger.LogGetTestCallResultForParticipant();
            
            var conference =
                await _queryHandler.Handle<GetConferenceByIdQuery, Conference>(
                    new GetConferenceByIdQuery(conferenceId));
            var videoPlatformService = _supplierPlatformServiceFactory.Create(conference.Supplier);
            var testCallResult = await videoPlatformService.GetTestCallScoreAsync(participantId);
            
            if (testCallResult == null)
            {
                _logger.LogSavingTestCallResult();
                return NotFound();
            }
            
            var command = new UpdateSelfTestCallResultCommand(conferenceId, participantId, testCallResult.Passed,
                testCallResult.Score);
            
            await _commandHandler.Handle(command);
            
            _logger.LogSavingTestCallResult();
            
            var response = TaskCallResultResponseMapper.MapTaskToResponse(testCallResult);
            
            return Ok(response);
        }
        
        /// <summary>
        /// Retrieves the independent self test result without saving it
        /// </summary>
        /// <param name="participantId">The id of the participant</param>
        /// <returns></returns>
        [HttpGet("independentselftestresult")]
        [OpenApiOperation("GetIndependentTestCallResult")]
        [ProducesResponseType(typeof(TestCallScoreResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetIndependentTestCallResultAsync(Guid participantId)
        {
            var videoPlatformService = _supplierPlatformServiceFactory.Create(Supplier.Vodafone);
            var testCallResult = await videoPlatformService.GetTestCallScoreAsync(participantId);
            if (testCallResult == null)
            {
                _logger.LogUnableToFindTestCallResult();
                return NotFound();
            }
            
            var response = TaskCallResultResponseMapper.MapTaskToResponse(testCallResult);
            return Ok(response);
        }
        
        /// <summary>
        /// Get the Heartbeat Data For Participant
        /// </summary>
        /// <param name="conferenceId">The id of the conference</param>
        /// <param name="participantId">The id of the participant</param>
        /// <returns></returns>
        [HttpGet("{conferenceId}/participant/{participantId}/heartbeatrecent")]
        [OpenApiOperation("GetHeartbeatDataForParticipant")]
        [ProducesResponseType(typeof(IEnumerable<ParticipantHeartbeatResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetHeartbeatDataForParticipantAsync(Guid conferenceId, Guid participantId)
        {
            _logger.LogGetHeartbeatDataForParticipant();
            
            var query = new GetHeartbeatsFromTimePointQuery(conferenceId, participantId, TimeSpan.FromMinutes(15));
            
            var heartbeats = await _queryHandler.Handle<GetHeartbeatsFromTimePointQuery, IList<Heartbeat>>(query);
            
            var responses =
                HeartbeatToParticipantHeartbeatResponseMapper.MapHeartbeatToParticipantHeartbeatResponse(heartbeats);
            
            return Ok(responses);
        }
        
        /// <summary>
        /// Post the Heartbeat Data For Participant
        /// </summary>
        /// <param name="conferenceId">The id of the conference</param>
        /// <param name="participantId">The id of the participant</param>
        /// <param name="request">The AddHeartbeatRequest</param>
        /// <returns></returns>
        [HttpPost("{conferenceId}/participant/{participantId}/heartbeat")]
        [OpenApiOperation("SaveHeartbeatDataForParticipant")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SaveHeartbeatDataForParticipantAsync(Guid conferenceId, Guid participantId,
            AddHeartbeatRequest request)
        {
            _logger.LogSaveHeartbeatDataForParticipant();
            
            if (request == null)
            {
                const string error = "AddHeartbeatRequest is null";
                _logger.LogAddHeartbeatRequestNull();
                ModelState.AddModelError(nameof(request), error);
                return ValidationProblem(ModelState);
            }
            
            var command = new SaveHeartbeatCommand
            (
                new Heartbeat(conferenceId, participantId, request.OutgoingAudioPercentageLost,
                    request.OutgoingAudioPercentageLostRecent, request.IncomingAudioPercentageLost,
                    request.IncomingAudioPercentageLostRecent, request.OutgoingVideoPercentageLost,
                    request.OutgoingVideoPercentageLostRecent, request.IncomingVideoPercentageLost,
                    request.IncomingVideoPercentageLostRecent, DateTime.UtcNow, request.BrowserName,
                    request.BrowserVersion,
                    request.OperatingSystem, request.OperatingSystemVersion, request.OutgoingAudioPacketsLost,
                    request.OutgoingAudioBitrate, request.OutgoingAudioCodec, request.OutgoingAudioPacketSent,
                    request.OutgoingVideoPacketSent, request.OutgoingVideoPacketsLost, request.OutgoingVideoFramerate,
                    request.OutgoingVideoBitrate, request.OutgoingVideoCodec, request.OutgoingVideoResolution,
                    request.IncomingAudioBitrate, request.IncomingAudioCodec, request.IncomingAudioPacketReceived,
                    request.IncomingAudioPacketsLost, request.IncomingVideoBitrate, request.IncomingVideoCodec,
                    request.IncomingVideoResolution, request.IncomingVideoPacketReceived,
                    request.IncomingVideoPacketsLost, request.Device)
            );
            
            await _commandHandler.Handle(command);
            
            return NoContent();
        }
        
        /// <summary>
        /// Get a list of participants for a given conference Id
        /// </summary>
        /// <param name="conferenceId">The conference Id</param>
        /// <returns>The list of participants</returns>
        [HttpGet("{conferenceId}/participants")]
        [OpenApiOperation("GetParticipantsByConferenceId")]
        [ProducesResponseType(typeof(List<ParticipantResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetParticipantsByConferenceId(Guid conferenceId)
        {
            _logger.LogGetParticipantsByConferenceId();
            
            var query = new GetConferenceByIdQuery(conferenceId);
            var conference = await _queryHandler.Handle<GetConferenceByIdQuery, Conference>(query);
            if (conference == null)
            {
                _logger.LogUnableToFindConference();
                return NotFound();
            }
            
            var participantRooms = conference.Rooms.OfType<ParticipantRoom>().ToList();
            var participants = conference.Participants.Select(x =>
            {
                var participantRoom =
                    participantRooms.SingleOrDefault(r => r.DoesParticipantExist(new RoomParticipant(x.Id)));
                return ParticipantResponseMapper.Map(x, participantRoom);
            }).ToList();
            return Ok(participants);
        }
        
        /// <summary>
        /// Add staff member to a conference
        /// </summary>
        /// <param name="conferenceId">The id of the conference to add participants to</param>
        /// <param name="request">Details of the participant</param>
        /// <returns></returns>
        [HttpPut("{conferenceId}/staffMember")]
        [OpenApiOperation("AddStaffMemberToConference")]
        [ProducesResponseType(typeof(AddStaffMemberResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddStaffMemberToConferenceAsync(Guid conferenceId,
            AddStaffMemberRequest request)
        {
            _logger.LogAddStaffMemberToConference();
            var participant = new Participant(request.DisplayName.Trim(), request.Username.ToLowerInvariant().Trim(),
                request.UserRole.MapToDomainEnum(),
                request.HearingRole, request.ContactEmail);
            try
            {
                var addParticipantCommand = new AddParticipantsToConferenceCommand(conferenceId,
                    new List<ParticipantBase>() { participant }, new List<LinkedParticipantDto>());
                
                await _commandHandler.Handle(addParticipantCommand);
                
                var response = new AddStaffMemberResponse
                {
                    ConferenceId = conferenceId,
                    Participant = ParticipantResponseMapper.Map(participant)
                };
                
                return Ok(response);
            }
            catch (ConferenceNotFoundException ex)
            {
                _logger.LogUnableToFindConferenceError(ex);
                return NotFound();
            }
        }
        
        /// <summary>
        /// Anonymise a participant with specified username
        /// </summary>
        /// <param name="username">username of participant</param>
        /// <returns></returns>
        [HttpPatch("username/{username}/anonymise-participant", Name = "AnonymiseParticipantWithUsername")]
        [OpenApiOperation("AnonymiseParticipantWithUsername")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> AnonymiseParticipantWithUsername(string username)
        {
            await _commandHandler.Handle(new AnonymiseParticipantWithUsernameCommand { Username = username });
            return Ok();
        }
        
        /// <summary>
        /// Anonymise a participant with associated expired conference
        /// </summary>
        /// <param name="request">hearing ids of expired conferences</param>
        /// <returns></returns>
        [HttpPatch("anonymise-quick-link-participant-with-hearing-ids",
            Name = "AnonymiseQuickLinkParticipantWithHearingIds")]
        [OpenApiOperation("AnonymiseQuickLinkParticipantWithHearingIds")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> AnonymiseQuickLinkParticipantWithHearingIds(
            AnonymiseQuickLinkParticipantWithHearingIdsRequest request)
        {
            await _commandHandler.Handle(new AnonymiseQuickLinkParticipantWithHearingIdsCommand
                { HearingIds = request.HearingIds });
            return Ok();
        }
        
        /// <summary>
        /// Update the username for a participant
        /// </summary>
        /// <param name="participantId">The id of the participant to update</param>
        /// <param name="request">New username to update to</param>
        /// <returns></returns>
        [HttpPatch("participants/{participantId}/username", Name = "UpdateParticipantUsername")]
        [OpenApiOperation("UpdateParticipantUsername")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateParticipantUsername(Guid participantId,
            UpdateParticipantUsernameRequest request)
        {
            try
            {
                await _commandHandler.Handle(new UpdateParticipantUsernameCommand(participantId, request.Username));
            }
            catch (ParticipantNotFoundException exception)
            {
                return NotFound(exception.Message);
            }
            
            return Ok();
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
            _logger.LogGetHostsInHearingsToday();
            
            var conferences =
                await _queryHandler.Handle<GetHostsInHearingsTodayQuery, List<Conference>>(
                    new GetHostsInHearingsTodayQuery());
            var results = conferences.SelectMany(HostInHearingResponseMapper.Map).ToList();
            return Ok(results);
        }
    }
}
