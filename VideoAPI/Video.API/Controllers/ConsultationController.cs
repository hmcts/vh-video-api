using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Services.Contracts;
using VideoApi.Services.Kinly;

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
        private readonly IConsultationService _consultationService;

        public ConsultationController(IQueryHandler queryHandler,
            ILogger<ConsultationController> logger, IVideoPlatformService videoPlatformService,
            IConsultationService consultationService, ICommandHandler commandHandler)
        {
            _queryHandler = queryHandler;
            _logger = logger;
            _videoPlatformService = videoPlatformService;
            _consultationService = consultationService;
            _commandHandler = commandHandler;
        }

        /// <summary>
        /// Raise or answer to a private consultation request with another participant
        /// </summary>
        /// <param name="request">Private consultation request with or without an answer</param>
        /// <returns></returns>
        [HttpPost]
        [SwaggerOperation(OperationId = "HandleConsultationRequest")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> HandleConsultationRequestAsync(ConsultationRequest request)
        {
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

            if (string.IsNullOrEmpty(request.RoomName))
            {
                _logger.LogWarning("Please provide a room name");
                return NotFound();
            }

            var command = new SaveEventCommand(conference.Id, Guid.NewGuid().ToString(), EventType.Consultation,
                DateTime.UtcNow, null, null, $"Adding {requestedFor.DisplayName} to {request.RoomName}", null)
            {
                ParticipantId = requestedBy.Id
            };

            await _commandHandler.Handle(command);

            await _consultationService.JoinConsultationRoomAsync(request.ConferenceId, requestedFor.Id, request.RoomName);

            return NoContent();
        }

        /// <summary>
        /// Leave a private consultation
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("leave")]
        [SwaggerOperation(OperationId = "LeavePrivateConsultation")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> LeavePrivateConsultationAsync(LeaveConsultationRequest request)
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

            var currentRoom = participant.CurrentRoom;
            if (!currentRoom.HasValue || currentRoom != RoomType.ConsultationRoom)
            {
                // This could only happen when both the participants press 'Close' button at the same time to end the call
                _logger.LogWarning("Participant is not in a consultation to leave from");
                return NoContent();
            }

            await _videoPlatformService.TransferParticipantAsync(conference.Id, participant.Id, participant.GetCurrentRoom(), RoomType.WaitingRoom);
            return NoContent();

        }

        /// <summary>
        /// Respond to a private consultation with a video hearings officer
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("vhofficer/respond")]
        [SwaggerOperation(OperationId = "RespondToAdminConsultationRequest")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> RespondToAdminConsultationRequestAsync(AdminConsultationRequest request)
        {
            const string modelErrorMessage = "Response to consultation is missing";
            var getConferenceByIdQuery = new GetConferenceByIdQuery(request.ConferenceId);
            var conference = await _queryHandler.Handle<GetConferenceByIdQuery, Conference>(getConferenceByIdQuery);

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
                    participant.GetCurrentRoom(), request.ConsultationRoom);
            }

            return NoContent();
        }

        /// <summary>
        /// Start a private consultation with a video endpoint
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("endpoint")]
        [SwaggerOperation(OperationId = "StartPrivateConsultationWithEndpoint")]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> StartPrivateConsultationWithEndpointAsync(EndpointConsultationRequest request)
        {
            var getConferenceByIdQuery = new GetConferenceByIdQuery(request.ConferenceId);
            var conference = await _queryHandler.Handle<GetConferenceByIdQuery, Conference>(getConferenceByIdQuery);

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
                var message = "Endpoint does not have a defence advocate linked";
                _logger.LogWarning(message);
                return Unauthorized(message);
            }

            if (!endpoint.DefenceAdvocate.Trim().Equals(defenceAdvocate.Username.Trim(), StringComparison.CurrentCultureIgnoreCase))
            {
                var message = "Defence advocate is not allowed to speak to requested endpoint";
                _logger.LogWarning(message);
                return Unauthorized(message);
            }

            var room = await _consultationService.CreateNewConsultationRoomAsync(request.ConferenceId, locked: true);
            await _consultationService.JoinConsultationRoomAsync(request.ConferenceId, defenceAdvocate.Id, room.Label);
            await _consultationService.JoinConsultationRoomAsync(request.ConferenceId, endpoint.Id, room.Label);

            return Accepted();
        }

        [HttpPost("start")]
        [SwaggerOperation(OperationId = "StartPrivateConsultation")]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> StartConsultationRequestAsync(StartConsultationRequest request)
        {
            try
            {
                var room = request.RoomType == VirtualCourtRoomType.Participant
                    ? await _consultationService.CreateNewConsultationRoomAsync(request.ConferenceId)
                    : await _consultationService.GetAvailableConsultationRoomAsync(request.ConferenceId, request.RoomType);
                await _consultationService.JoinConsultationRoomAsync(request.ConferenceId, request.RequestedBy, room.Label);

                return Accepted();
            }
            catch (ConferenceNotFoundException ex)
            {
                _logger.LogError(ex,
                    "Cannot create consultation for conference: {conferenceId} as the conference does not exist",
                    request.ConferenceId);
                return NotFound("Conference does not exist");
            }
            catch (ParticipantNotFoundException ex)
            {
                _logger.LogError(ex,
                    "Cannot create consultation with participant: {participantId} as the participant does not exist",
                    request.RequestedBy);
                return NotFound("Participant doesn't exist");
            }
            catch (KinlyApiException ex)
            {
                _logger.LogError(ex,
                    "Unable to create a consultation room for ConferenceId: {conferenceId}",
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
        [SwaggerOperation(OperationId = "LeaveConsultationAsync")]
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
    }
}
