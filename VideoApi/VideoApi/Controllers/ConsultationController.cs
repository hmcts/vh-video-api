using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Domain.Validations;
using VideoApi.Extensions;
using VideoApi.Services.Contracts;
using VideoApi.Services.Kinly;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.Controllers
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
        private readonly IConsultationService _consultationService;

        public ConsultationController(IQueryHandler queryHandler, ICommandHandler commandHandler,
            ILogger<ConsultationController> logger, IVideoPlatformService videoPlatformService,
            IConsultationService consultationService)
        {
            _queryHandler = queryHandler;
            _commandHandler = commandHandler;
            _logger = logger;
            _videoPlatformService = videoPlatformService;
            _consultationService = consultationService;
        }

        /// <summary>
        /// Raise or answer to a private consultation request with another participant
        /// </summary>
        /// <param name="request">Private consultation request with or without an answer</param>
        /// <returns></returns>
        [HttpPost]
        [OpenApiOperation("HandleConsultationRequest")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> HandleConsultationRequestAsync(ConsultationRequest request)
        {
            _logger.LogDebug("HandleConsultationRequest");
            var getConferenceByIdQuery = new GetConferenceByIdQuery(request.ConferenceId);
            var conference = await _queryHandler.Handle<GetConferenceByIdQuery, Conference>(getConferenceByIdQuery);
            if (conference == null)
            {
                _logger.LogWarning("Unable to find conference");
                return NotFound();
            }

            var requestedBy = conference.GetParticipants().SingleOrDefault(x => x.Id == request.RequestedBy);
            if (requestedBy == null)
            {
                _logger.LogWarning("Unable to find participant request by with id {RequestedBy}", request.RequestedBy);
                return NotFound();
            }

            var requestedFor = conference.GetParticipants().SingleOrDefault(x => x.Id == request.RequestedFor);
            if (requestedFor == null)
            {
                _logger.LogWarning("Unable to find participant request for with id {RequestedFor}", request.RequestedFor);
                return NotFound();
            }

            var command = new SaveEventCommand(conference.Id, Guid.NewGuid().ToString(), EventType.Consultation,
                DateTime.UtcNow, null, null, $"Consultation with {requestedFor.DisplayName}", null)
            {
                ParticipantId = requestedBy.Id
            };
            await _commandHandler.Handle(command);

            try
            {
                await InitiateStartConsultationAsync(conference, requestedBy, requestedFor, request.Answer.GetValueOrDefault());
            }
            catch (DomainRuleException ex)
            {
                _logger.LogError(ex, "No consultation room available for conference");
                ModelState.AddModelError("ConsultationRoom", "No consultation room available");
                return BadRequest(ModelState);
            }

            return NoContent();
        }

        /// <summary>
        /// Leave a private consultation
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("leave")]
        [OpenApiOperation("LeavePrivateConsultation")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> LeavePrivateConsultationAsync(LeaveConsultationRequest request)
        {
            _logger.LogDebug("LeavePrivateConsultation");
            var getConferenceByIdQuery = new GetConferenceByIdQuery(request.ConferenceId);
            var conference = await _queryHandler.Handle<GetConferenceByIdQuery, Conference>(getConferenceByIdQuery);

            if (conference == null)
            {
                _logger.LogWarning("Unable to find conference");
                return NotFound();
            }

            var participant = conference.GetParticipants().SingleOrDefault(x => x.Id == request.ParticipantId);
            if (participant == null)
            {
                _logger.LogWarning("Unable to find participant request by id");
                return NotFound();
            }

            var currentRoom = participant.CurrentRoom;
            if (!currentRoom.HasValue || (currentRoom != RoomType.ConsultationRoom1 &&
                                          currentRoom != RoomType.ConsultationRoom2))
            {
                // This could only happen when both the participants press 'Close' button at the same time to end the call
                _logger.LogWarning("Participant is not in a consultation to leave from");
                return NoContent();
            }

            await _videoPlatformService.StopPrivateConsultationAsync(conference, currentRoom.Value);
            return NoContent();

        }

        /// <summary>
        /// Respond to a private consultation with a video hearings officer
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("vhofficer/respond")]
        [OpenApiOperation("RespondToAdminConsultationRequest")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> RespondToAdminConsultationRequestAsync(AdminConsultationRequest request)
        {
            _logger.LogDebug("RespondToAdminConsultationRequest");

            const string modelErrorMessage = "Response to consultation is missing";
            var getConferenceByIdQuery = new GetConferenceByIdQuery(request.ConferenceId);
            var conference =
                await _queryHandler.Handle<GetConferenceByIdQuery, Conference>(getConferenceByIdQuery);

            if (!request.Answer.HasValue)
            {
                ModelState.AddModelError(nameof(request.Answer), modelErrorMessage);
                return BadRequest(ModelState);
            }

            if (conference == null)
            {
                _logger.LogWarning("Unable to find conference");
                return NotFound();
            }

            var participant = conference.GetParticipants().SingleOrDefault(x => x.Id == request.ParticipantId);
            if (participant == null)
            {
                _logger.LogWarning("Unable to find participant request by id");
                return NotFound();
            }

            if (request.Answer.Value == ConsultationAnswer.Accepted)
            {
                await _videoPlatformService.TransferParticipantAsync(conference.Id, participant.Id,
                    participant.GetCurrentRoom(), request.ConsultationRoom.MapToDomainEnum());
            }

            return NoContent();
        }

        /// <summary>
        /// Start a private consultation with a video endpoint
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("endpoint")]
        [OpenApiOperation("StartPrivateConsultationWithEndpoint")]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> StartPrivateConsultationWithEndpointAsync(EndpointConsultationRequest request)
        {
            _logger.LogDebug("StartPrivateConsultationWithEndpoint");

            var getConferenceByIdQuery = new GetConferenceByIdQuery(request.ConferenceId);
            var conference =
                await _queryHandler.Handle<GetConferenceByIdQuery, Conference>(getConferenceByIdQuery);

            if (conference == null)
            {
                _logger.LogWarning("Unable to find conference");
                return NotFound($"Unable to find conference {request.ConferenceId}");
            }

            var endpoint = conference.GetEndpoints().SingleOrDefault(x => x.Id == request.EndpointId);
            if (endpoint == null)
            {
                _logger.LogWarning("Unable to find endpoint");
                return NotFound($"Unable to find endpoint {request.EndpointId}");
            }

            var defenceAdvocate = conference.GetParticipants().SingleOrDefault(x => x.Id == request.DefenceAdvocateId);
            if (defenceAdvocate == null)
            {
                _logger.LogWarning("Unable to find defence advocate");
                return NotFound($"Unable to find defence advocate {request.DefenceAdvocateId}");
            }

            if (string.IsNullOrWhiteSpace(endpoint.DefenceAdvocate))
            {
                const string message = "Endpoint does not have a defence advocate linked";
                _logger.LogWarning(message);
                return Unauthorized(message);
            }

            if (!endpoint.DefenceAdvocate.Trim().Equals(defenceAdvocate.Username.Trim(), StringComparison.CurrentCultureIgnoreCase))
            {
                const string message = "Defence advocate is not allowed to speak to requested endpoint";
                _logger.LogWarning(message);
                return Unauthorized(message);
            }

            await _videoPlatformService.StartEndpointPrivateConsultationAsync(conference, endpoint, defenceAdvocate);

            return Accepted();
        }

        [HttpPost("start")]
        [OpenApiOperation("StartPrivateConsultation")]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> StartConsultationRequestAsync(StartConsultationRequest request)
        {
            try
            {
                var room = await _consultationService.GetAvailableConsultationRoomAsync(request.ConferenceId,
                    request.RoomType.MapToDomainEnum());
                await _consultationService.JoinConsultationRoomAsync(request.ConferenceId, request.RequestedBy, room.Label);

                return Accepted();
            }
            catch (ConferenceNotFoundException ex)
            {
                _logger.LogError(ex,
                    "Cannot create consultation for conference: {ConferenceId} as the conference does not exist",
                    request.ConferenceId);
                return NotFound("Conference does not exist");
            }
            catch (ParticipantNotFoundException ex)
            {
                _logger.LogError(ex,
                    "Cannot create consultation with participant: {ParticipantId} as the participant does not exist",
                    request.RequestedBy);
                return NotFound("Participant doesn't exist");
            }
            catch (KinlyApiException ex)
            {
                _logger.LogError(ex,
                    "Unable to create a consultation room for ConferenceId: {ConferenceId}",
                    request.ConferenceId);
                return BadRequest("Consultation room creation failed");
            }
        }

        /// <summary>
        /// Leave a consultation.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("end")]
        [OpenApiOperation("LeaveConsultationAsync")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> LeaveConsultationAsync(LeaveConsultationRequest request)
        {
            var getConferenceByIdQuery = new GetConferenceByIdQuery(request.ConferenceId);
            var conference = await _queryHandler.Handle<GetConferenceByIdQuery, Conference>(getConferenceByIdQuery);
            if (conference == null)
            {
                _logger.LogWarning("Unable to find conference");
                return NotFound();
            }
            var participant = conference.GetParticipants().SingleOrDefault(x => x.Id == request.ParticipantId);
            if (participant == null)
            {
                _logger.LogWarning("Unable to find participant request by id");
                return NotFound();
            }

            try
            {
                var currentRoom = participant.CurrentVirtualRoom.Label;
                await _consultationService.LeaveConsultationAsync(request.ConferenceId, request.ParticipantId, currentRoom,
                    VirtualCourtRoomType.WaitingRoom.ToString());
                return Ok();
            }
            catch (KinlyApiException ex)
            {
                _logger.LogError(ex,
                    "Unable to leave a consultation room for ConferenceId: {conferenceId}",
                    request.ConferenceId);
                return BadRequest("Error on Leave Consultation room");
            }
        }

        private async Task InitiateStartConsultationAsync(Conference conference, Participant requestedBy,
            Participant requestedFor, ConsultationAnswer answer)
        {
            if (answer == ConsultationAnswer.Accepted)
            {
                _logger.LogInformation(
                    "Conference: {ConferenceId} - Attempting to start private consultation between {RequestedById} and {RequestedForId}",
                    conference.Id, requestedBy.Id, requestedFor.Id);
                await _videoPlatformService.StartPrivateConsultationAsync(conference, requestedBy, requestedFor);
            }
        }
    }
}
