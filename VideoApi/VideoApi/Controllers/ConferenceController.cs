using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using VideoApi.Contract.Enums;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Extensions;
using VideoApi.Mappings;
using VideoApi.Services;
using VideoApi.Services.Contracts;
using VideoApi.Services.Dtos;
using VideoApi.Services.Mappers;
using VideoApi.Validations;
using Task = System.Threading.Tasks.Task;
using ConferenceRoomType = VideoApi.Contract.Enums.ConferenceRoomType;
using VideoApi.Common.Logging;

namespace VideoApi.Controllers;

[Consumes("application/json")]
[Produces("application/json")]
[Route("conferences")]
[ApiController]
public class ConferenceController(
    IQueryHandler queryHandler,
    ICommandHandler commandHandler,
    ISupplierPlatformServiceFactory supplierPlatformServiceFactory,
    ILogger<ConferenceController> logger,
    IAudioPlatformService audioPlatformService,
    IPollyRetryService pollyRetryService,
    IBookingService bookingService)
    : ControllerBase
{

    /// <summary>
    /// Request to book a conference
    /// </summary>
    /// <param name="request">Details of a conference</param>
    /// <returns>Details of the new conference</returns>
    [HttpPost]
    [OpenApiOperation("BookNewConference")]
    [ProducesResponseType(typeof(ConferenceDetailsResponse), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> BookNewConferenceAsync(BookNewConferenceRequest request)
    {
        logger.LogBookNewConference();

        foreach (var participant in request.Participants)
        {
            participant.Username = participant.Username.ToLower().Trim();
            participant.DisplayName = participant.DisplayName.Trim();
        }

        var audioIngestUrl = audioPlatformService.GetAudioIngestUrl(request.CaseTypeServiceId, request.CaseNumber, request.HearingRefId.ToString());

        var conferenceId = await CreateConferenceWithRetiesAsync(request, audioIngestUrl);
        logger.LogConferenceCreatedDebug();

        var conferenceEndpoints = await queryHandler.Handle<GetEndpointsForConferenceQuery, IList<Endpoint>>(new GetEndpointsForConferenceQuery(conferenceId));
        var endpointDtos = conferenceEndpoints.Select(EndpointMapper.MapToEndpoint);

        var roomBookedSuccess = await BookMeetingRoomWithRetriesAsync(conferenceId, request.AudioRecordingRequired,
            audioIngestUrl, endpointDtos, request.ConferenceRoomType, request.AudioPlaybackLanguage,
            request.Supplier);

        if (!roomBookedSuccess)
        {
            var message = $"Could not book and find meeting room for conferenceId: {conferenceId}";
            logger.LogRoomBookingFailed(conferenceId);
            return StatusCode((int)HttpStatusCode.InternalServerError, message);
        }

        logger.LogRoomBooked();

        var getConferenceByIdQuery = new GetConferenceByIdQuery(conferenceId);
        var queriedConference = await queryHandler.Handle<GetConferenceByIdQuery, Conference>(getConferenceByIdQuery);

        var supplierPlatformService = supplierPlatformServiceFactory.Create((Domain.Enums.Supplier)request.Supplier);
        var supplierConfiguration = supplierPlatformService.GetSupplierConfiguration();
        var response = ConferenceToDetailsResponseMapper.Map(queriedConference, supplierConfiguration);

        logger.LogConferenceCreatedInfo(response.Id, request.HearingRefId);

        return CreatedAtAction("GetConferenceDetailsById", new { conferenceId = response.Id }, response);
    }

    /// <summary>
    /// Updates a conference
    /// </summary>
    /// <param name="request">Details of a conference</param>
    /// <returns>Ok status</returns>
    [HttpPut]
    [OpenApiOperation("UpdateConference")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> UpdateConferenceAsync(UpdateConferenceRequest request)
    {
        logger.LogUpdateConference();

        var query = new GetNonClosedConferenceByHearingRefIdQuery(request.HearingRefId);
        var conferencesList = await queryHandler.Handle<GetNonClosedConferenceByHearingRefIdQuery, List<Conference>>(query);
        var conference = conferencesList.FirstOrDefault();
        if (conference == null)
        {
            logger.LogConferenceNotFoundWarning(request.HearingRefId);
            return NotFound();
        }

        var endpointDtos = conference.GetEndpoints().Select(EndpointMapper.MapToEndpoint);
        var videoPlatformService = supplierPlatformServiceFactory.Create(conference.Supplier);
        await videoPlatformService.UpdateVirtualCourtRoomAsync(conference.Id, request.AudioRecordingRequired,
            endpointDtos, request.RoomType.MapToDomainEnum(), request.AudoAudioPlaybackLanguage.MapToDomainEnum());

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
    [ProducesResponseType(typeof(ConferenceDetailsResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetConferenceDetailsByIdAsync(Guid conferenceId)
    {
        logger.LogGetConferenceDetailsById(conferenceId);

        var getConferenceByIdQuery = new GetConferenceByIdQuery(conferenceId);
        var queriedConference = await queryHandler.Handle<GetConferenceByIdQuery, Conference>(getConferenceByIdQuery);

        if (queriedConference == null)
        {
            logger.LogConferenceNotFound(conferenceId);
            return NotFound($"Unable to find a conference with the given id {conferenceId}");
        }

        var supplierPlatformService = supplierPlatformServiceFactory.Create(queriedConference.Supplier);
        var supplierConfiguration = supplierPlatformService.GetSupplierConfiguration();
        var response = ConferenceToDetailsResponseMapper.Map(queriedConference, supplierConfiguration);
        return Ok(response);
    }

    /// <summary>
    /// Remove an existing conference
    /// </summary>
    /// <param name="conferenceId">The conference id</param>
    /// <returns></returns>
    [HttpDelete("{conferenceId}")]
    [OpenApiOperation("RemoveConference")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> RemoveConferenceAsync(Guid conferenceId)
    {
        logger.LogRemoveConference(conferenceId);
        var removeConferenceCommand = new RemoveConferenceCommand(conferenceId);
        try
        {
            var conference = await queryHandler.Handle<GetConferenceByIdQuery, Conference>(new GetConferenceByIdQuery(conferenceId));
            await commandHandler.Handle(removeConferenceCommand);
            await SafelyRemoveCourtRoomAsync(conferenceId, conference.Supplier);

            logger.LogConferenceRemoved(conferenceId);

            return NoContent();
        }
        catch (ConferenceNotFoundException ex)
        {
            logger.LogRemoveConferenceError(ex, conferenceId);
            return NotFound();
        }
    }

    /// <summary>
    /// Get non-closed conferences for a participant by their username
    /// </summary>
    /// <param name="username">person username</param>
    /// <returns>List of non-closed conferences for judge</returns>
    [HttpGet("today/individual")]
    [OpenApiOperation("GetConferencesTodayForIndividualByUsername")]
    [ProducesResponseType(typeof(List<ConferenceCoreResponse>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetConferencesTodayForIndividualByUsernameAsync([FromQuery] string username)
    {
        logger.LogGetConferencesTodayForIndividual(username);

        if (!username.IsValidEmail())
        {
            ModelState.AddModelError(nameof(username), $"Please provide a valid {nameof(username)}");
            logger.LogInvalidUsername(username);
            return ValidationProblem(ModelState);
        }

        var query = new GetConferencesForTodayByIndividualQuery(username.ToLower().Trim());
        var conferences = await queryHandler.Handle<GetConferencesForTodayByIndividualQuery, List<Conference>>(query);
        return Ok(conferences.Select(ConferenceCoreResponseMapper.Map));
    }


    /// <summary>
    /// Get conferences by hearing ref ids
    /// </summary>
    /// <param name="request">Hearing IDs within GetConferencesByHearingIdsRequest</param>
    /// <returns>List of Base conference core objects</returns>
    [HttpPost("hearings")]
    [OpenApiOperation("GetConferencesByHearingRefIds")]
    [ProducesResponseType(typeof(List<ConferenceCoreResponse>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetConferencesByHearingRefIdsAsync(GetConferencesByHearingIdsRequest request)
    {
        if(!ValidateGetConferencesByHearingIdsRequest(request))
            return ValidationProblem(ModelState);
        
        var query = new GetNonClosedConferenceByHearingRefIdQuery(request.HearingRefIds, request.IncludeClosed);
        
        var conferencesList =
            await queryHandler.Handle<GetNonClosedConferenceByHearingRefIdQuery, List<Conference>>(query);
        
        if (conferencesList == null || conferencesList.Count == 0)
            return NotFound();
        
        return Ok(conferencesList.Select(ConferenceCoreResponseMapper.Map));
    }
    
    
    /// <summary>
    /// Get full conference details by hearing ref ids
    /// </summary>
    /// <param name="request">Hearing IDs within GetConferencesByHearingIdsRequest</param>
    /// <returns>List of conferences with full details including participants and statuses of a conference</returns>
    [HttpPost("hearings/details")]
    [OpenApiOperation("GetConferenceDetailsByHearingRefIds")]
    [ProducesResponseType(typeof(List<ConferenceDetailsResponse>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetConferenceDetailsByHearingRefIdsAsync(GetConferencesByHearingIdsRequest request)
    {
        if(!ValidateGetConferencesByHearingIdsRequest(request))
            return ValidationProblem(ModelState);
            
        var query = new GetNonClosedConferenceByHearingRefIdQuery(request.HearingRefIds, request.IncludeClosed);
        
        var conferencesList =
            await queryHandler.Handle<GetNonClosedConferenceByHearingRefIdQuery, List<Conference>>(query);
        
        if (conferencesList == null || conferencesList.Count == 0)
            return NotFound();
        
        var supplierConfigMapper = new SupplierConfigurationMapper(supplierPlatformServiceFactory);
        var supplierConfigs = supplierConfigMapper.ExtractSupplierConfigurations(conferencesList);
        
        var response = conferencesList.Select(c =>
        {
            var supplierConfig = supplierConfigs.Find(sc => sc.Supplier == c.Supplier);
            return ConferenceToDetailsResponseMapper.Map(c, supplierConfig.Configuration);
        });
        return Ok(response);
    }
    
    private bool ValidateGetConferencesByHearingIdsRequest(GetConferencesByHearingIdsRequest request)
    {
        if (request.HearingRefIds == null || request.HearingRefIds.Length == 0 || Array.Exists(request.HearingRefIds,x => x.Equals(Guid.Empty)))
        {
            ModelState.AddModelError(nameof(request.HearingRefIds), "Please provide at least one hearing id");
            return false;
        }
        return true;
    }
    
    /// <summary>
    /// Get list of expired conferences 
    /// </summary>
    /// <returns>Conference summary details</returns>
    [HttpGet("expired")]
    [OpenApiOperation("GetExpiredOpenConferences")]
    [ProducesResponseType(typeof(List<ExpiredConferencesResponse>), (int)HttpStatusCode.OK)]
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
    [ProducesResponseType(typeof(List<ExpiredConferencesResponse>), (int)HttpStatusCode.OK)]
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
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CloseConferenceAsync(Guid conferenceId)
    {
        try
        {
            var command = new CloseConferenceCommand(conferenceId);
            
            var conference =
                await queryHandler.Handle<GetConferenceByIdQuery, Conference>(
                    new GetConferenceByIdQuery(conferenceId));
            await commandHandler.Handle(command);
            await SafelyRemoveCourtRoomAsync(conferenceId, conference.Supplier);
            
            return NoContent();
        }
        catch (ConferenceNotFoundException ex)
        {
            logger.LogError(ex, "Unable to find conference {ConferenceId}", conferenceId);
            
            return NotFound();
        }
    }
    
    /// <summary>
    /// Get conferences Hearing rooms
    /// </summary>
    /// <returns>Hearing rooms details</returns>
    [HttpGet("hearingRooms")]
    [OpenApiOperation("GetConferencesHearingRooms")]
    [ProducesResponseType(typeof(List<ConferenceHearingRoomsResponse>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> GetConferencesHearingRoomsAsync([FromQuery] string date)
    {
        logger.LogDebug("GetConferencesHearingRooms");
        
        try
        {
            var requestedDate = DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            var conferences =
                await queryHandler.Handle<GetConferenceHearingRoomsByDateQuery, List<HearingAudioRoom>>(
                    new GetConferenceHearingRoomsByDateQuery(requestedDate));
            
            var response = ConferenceHearingRoomsResponseMapper.Map(conferences, requestedDate);
            
            return Ok(response);
        }
        catch (Exception e)
        {
#pragma warning disable CA2254 // Template should be a static expression
            logger.LogError(e, e.Message);
#pragma warning restore CA2254 // Template should be a static expression
            return NoContent();
        }
    }
    
    /// <summary>
    /// Anonymises the Conference and Participant data.
    /// </summary>
    /// <returns></returns>
    [HttpPatch("anonymiseconferences")]
    [OpenApiOperation("AnonymiseConferences")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> AnonymiseConferencesAsync()
    {
        logger.LogDebug("AnonymiseConferencesAndParticipantInformation");
        
        var anonymiseConferenceCommand = new AnonymiseConferencesCommand();
        await commandHandler.Handle(anonymiseConferenceCommand);
        
        logger.LogInformation("Records updated: {RecordsUpdated}", anonymiseConferenceCommand.RecordsUpdated);
        return NoContent();
    }
    
    /// <summary>
    /// Anonymise conference with matching hearing ids
    /// </summary>
    /// <param name="request">hearing ids of expired conferences</param>
    /// <returns></returns>
    [HttpPatch("anonymise-conference-with-hearing-ids")]
    [OpenApiOperation("AnonymiseConferenceWithHearingIds")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> AnonymiseConferenceWithHearingIds(
        AnonymiseConferenceWithHearingIdsRequest request)
    {
        await commandHandler.Handle(new AnonymiseConferenceWithHearingIdsCommand
            { HearingIds = request.HearingIds });
        return Ok();
    }
    
    /// <summary>
    /// Get today's conferences, optionally filtered by hearing venue names
    /// </summary>
    /// <returns>Conference details</returns>
    [HttpGet("today")]
    [OpenApiOperation("GetConferencesToday")]
    [ProducesResponseType(typeof(List<ConferenceDetailsResponse>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetConferencesToday([FromQuery] ConferenceTodayRequest request)
    {
        var conferences = await queryHandler.Handle<GetConferencesTodayQuery, List<Conference>>(new GetConferencesTodayQuery
        {
            HearingVenueNames = request.HearingVenueNames
        });
        var conferencesForToday = new List<ConferenceDetailsResponse>();
        foreach (var conference in conferences)
        {
            var supplierPlatformService = supplierPlatformServiceFactory.Create(conference.Supplier);
            var supplierConfiguration = supplierPlatformService.GetSupplierConfiguration();
            conferencesForToday.Add(ConferenceToDetailsResponseMapper.Map(conference, supplierConfiguration));
        }
        return Ok(conferencesForToday);
    }

    private async Task<bool> BookMeetingRoomWithRetriesAsync(Guid conferenceId,
        bool audioRecordingRequired,
        string ingestUrl,
        IEnumerable<EndpointDto> endpoints,
        ConferenceRoomType roomType, AudioPlaybackLanguage audioPlaybackLanguage = AudioPlaybackLanguage.EnglishAndWelsh, Supplier supplier = Supplier.Vodafone) => await pollyRetryService.WaitAndRetryAsync<Exception, bool>
    (
        3,
        _ => TimeSpan.FromSeconds(10),
        retryAttempt =>
            logger.LogRetryBookingMeetingRoom(retryAttempt),
        callResult => !callResult,
        async () => await bookingService.BookMeetingRoomAsync(conferenceId, audioRecordingRequired, ingestUrl,
            endpoints, roomType.MapToDomainEnum(), audioPlaybackLanguage.MapToDomainEnum(), supplier.MapToDomainEnum())
    );


    private async Task<Guid> CreateConferenceWithRetiesAsync(BookNewConferenceRequest request, string ingestUrl) =>
        await pollyRetryService.WaitAndRetryAsync<Exception, Guid>
        (
            3,
            _ => TimeSpan.FromSeconds(10),
            retryAttempt => logger.LogRetryCreateConference(retryAttempt),
            callResult => callResult == Guid.Empty,
            async () => await bookingService.CreateConferenceAsync(request, ingestUrl)
        );

    private async Task SafelyRemoveCourtRoomAsync(Guid conferenceId, VideoApi.Domain.Enums.Supplier supplier)
    {
        var videoPlatformService = supplierPlatformServiceFactory.Create(supplier);
        var meetingRoom = await videoPlatformService.GetVirtualCourtRoomAsync(conferenceId);
        if (meetingRoom != null)
        {
            await videoPlatformService.DeleteVirtualCourtRoomAsync(conferenceId);
        }
    }
}
