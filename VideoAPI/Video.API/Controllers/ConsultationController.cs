using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Polly;
using Swashbuckle.AspNetCore.Annotations;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Domain.Validations;
using VideoApi.Services;
using Task = System.Threading.Tasks.Task;

namespace Video.API.Controllers
{
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("consultations")]
    [ApiController]
    public class ConsultationController : ControllerBase
    {
        private readonly IQueryHandler _queryHandler;
        private readonly ICommandHandler _commandHandler;
        private readonly ILogger<ConsultationController> _logger;
        private readonly IVideoPlatformService _videoPlatformService;
        private readonly IConsultationCache _consultationCache;

        public ConsultationController(IQueryHandler queryHandler, ICommandHandler commandHandler,
            ILogger<ConsultationController> logger,
            IVideoPlatformService videoPlatformService,
            IConsultationCache consultationCache)
        {
            _queryHandler = queryHandler;
            _commandHandler = commandHandler;
            _logger = logger;
            _videoPlatformService = videoPlatformService;
            _consultationCache = consultationCache;
        }

        /// <summary>
        /// Raise or answer to a private consultation request with another participant
        /// </summary>
        /// <param name="request">Private consultation request with or without an answer</param>
        /// <returns></returns>
        [HttpPost]
        [SwaggerOperation(OperationId = "HandleConsultationRequest")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> HandleConsultationRequest(ConsultationRequest request)
        {
            _logger.LogDebug($"HandleConsultationRequest");
            var conference = await GetConference(request.ConferenceId);

            if (conference == null)
            {
                _logger.LogError($"Unable to find conference {request.ConferenceId}");
                return NotFound();
            }

            var requestedBy = conference.GetParticipants().SingleOrDefault(x => x.Id == request.RequestedBy);
            if (requestedBy == null)
            {
                _logger.LogError($"Unable to find participant request by with id {request.RequestedBy}");
                return NotFound();
            }

            var requestedFor = conference.GetParticipants().SingleOrDefault(x => x.Id == request.RequestedFor);
            if (requestedFor == null)
            {
                _logger.LogError($"Unable to find participant request for with id {request.RequestedFor}");
                return NotFound();
            }

            var command = new SaveEventCommand(conference.Id, Guid.NewGuid().ToString(), EventType.Consultation,
                DateTime.UtcNow, null, null, $"Consultation with {requestedFor.DisplayName}")
            {
                ParticipantId = requestedBy.Id
            };
            await _commandHandler.Handle(command);

            try
            {
             await InitiateStartConsultation(conference, requestedBy, requestedFor,
                    request.Answer.GetValueOrDefault());
            }
            catch (DomainRuleException e)
            {
                _logger.LogError(e, $"No consultation room available for conference {conference.Id}");
                ModelState.AddModelError("ConsultationRoom", "No consultation room available");
                return BadRequest(ModelState);
            }

            return NoContent();
        }

        [HttpPost("leave")]
        [SwaggerOperation(OperationId = "LeavePrivateConsultation")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> LeavePrivateConsultation(LeaveConsultationRequest request)
        {
            _logger.LogDebug($"LeavePrivateConsultation");

            var getConferenceByIdQuery = new GetConferenceByIdQuery(request.ConferenceId);
            var conference =
                await _queryHandler.Handle<GetConferenceByIdQuery, Conference>(getConferenceByIdQuery);

            if (conference == null)
            {
                _logger.LogError($"Unable to find conference {request.ConferenceId}");
                return NotFound();
            }

            var participant = conference.GetParticipants().SingleOrDefault(x => x.Id == request.ParticipantId);
            if (participant == null)
            {
                _logger.LogError($"Unable to find participant request by with id {request.ParticipantId}");
                return NotFound();
            }

            var currentRoom = participant.CurrentRoom;
            if (!currentRoom.HasValue || (currentRoom != RoomType.ConsultationRoom1 &&
                                          currentRoom != RoomType.ConsultationRoom2))
            {
                _logger.LogError($"Participant {request.ParticipantId} is not in a consultation to leave from");
                ModelState.AddModelError("Room",
                    $"Participant {request.ParticipantId} is not in a consultation room");
                return BadRequest(ModelState);
            }

            await _videoPlatformService.StopPrivateConsultationAsync(conference, currentRoom.Value);

            return NoContent();
        }

        [HttpPost("vhofficer/respond")]
        [SwaggerOperation(OperationId = "RespondToAdminConsultationRequest")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> RespondToAdminConsultationRequest(AdminConsultationRequest request)
        {
            _logger.LogDebug($"RespondToAdminConsultationRequest");
            
            var getConferenceByIdQuery = new GetConferenceByIdQuery(request.ConferenceId);
            var conference =
                await _queryHandler.Handle<GetConferenceByIdQuery, Conference>(getConferenceByIdQuery);

            if (conference == null)
            {
                _logger.LogError($"Unable to find conference {request.ConferenceId}");
                return NotFound();
            }

            var participant = conference.GetParticipants().SingleOrDefault(x => x.Id == request.ParticipantId);
            if (participant == null)
            {
                _logger.LogError($"Unable to find participant request by with id {request.ParticipantId}");
                return NotFound();
            }

            if (request.Answer.Value == ConsultationAnswer.Accepted)
            {
                await _videoPlatformService.TransferParticipantAsync(conference.Id, participant.Id,
                    participant.CurrentRoom.Value, request.ConsultationRoom);
            }

            return NoContent();
        }
        
        private async Task InitiateStartConsultation(Conference conference, Participant requestedBy,
            Participant requestedFor, ConsultationAnswer answer)
        {
            if (answer == ConsultationAnswer.Accepted)
            {
                _logger.LogInformation(
                    $"Conference: {conference.Id} - Attempting to start private consultation between {requestedBy.Id} and {requestedFor.Id}");

                RoomType? roomInCache = await _consultationCache.GetConsultationRoom(conference.Id);
                
                if (roomInCache.HasValue)
                {
                    // Retry until the cache is removed from the previous request
                    var retryPolicy = Policy
                    .HandleResult<RoomType?>(room => !room.HasValue)
                    .WaitAndRetryAsync(3, x => TimeSpan.FromSeconds(1));

                    await retryPolicy.ExecuteAsync(() => _consultationCache.GetConsultationRoom(conference.Id));

                    // Get a fresh conference record again
                    conference = await GetConference(conference.Id);
                }
                else
                {
                    // If there is nothing in the cache, then add this room to the cache
                    var targetRoom = conference.GetAvailableConsultationRoom();
                    await _consultationCache.AddConsultationRoomToCache(conference.Id, targetRoom);
                }

                await _videoPlatformService.StartPrivateConsultationAsync(conference, requestedBy, requestedFor);

                // Remove from the cache
                _consultationCache.Remove(conference.Id);
            }
        }

        private async Task<Conference> GetConference(Guid conferenceId)
        {
            var getConferenceByIdQuery = new GetConferenceByIdQuery(conferenceId);
            return await _queryHandler.Handle<GetConferenceByIdQuery, Conference>(getConferenceByIdQuery);
        }
    }
}
