using System;
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
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Extensions;
using VideoApi.Mappings;
using VideoApi.Services.Contracts;
using VideoApi.Common.Logging;

namespace VideoApi.Controllers
{
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("consultations")]
    [ApiController]
    public class ConsultationController(
        IQueryHandler queryHandler,
        ILogger<ConsultationController> logger,
        IConsultationService consultationService,
        ICommandHandler commandHandler)
        : ControllerBase
    {
        /// <summary>
        /// Raise or answer to a private consultation request with another participant
        /// </summary>
        /// <param name="request">Private consultation request with or without an answer</param>
        /// <returns></returns>
        [HttpPost]
        [OpenApiOperation("RespondToConsultationRequestAsync")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> RespondToConsultationRequestAsync(ConsultationRequestResponse request)
        {
            var getConferenceByIdQuery = new GetConferenceByIdQuery(request.ConferenceId);
            var conference = await queryHandler.Handle<GetConferenceByIdQuery, Conference>(getConferenceByIdQuery);
            if (conference == null)
            {
                logger.LogConferenceNotFound(request.ConferenceId);
                return NotFound($"Unable to find conference {request.ConferenceId}");
            }

            var requestedBy = conference.GetParticipants().SingleOrDefault(x => x.Id == request.RequestedBy);
            if (request.RequestedBy != Guid.Empty && requestedBy == null)
            {
                logger.LogParticipantRequestedByNotFound(request.RequestedBy);
                return NotFound();
            }
            
            var requestedFor = conference.GetParticipants().SingleOrDefault(x => x.Id == request.RequestedFor);
            if (requestedFor == null)
            {
                logger.LogParticipantRequestedForNotFound(request.RequestedFor);
                return NotFound($"Unable to find participant id request for {request.RequestedFor}");
            }

            if (string.IsNullOrEmpty(request.RoomLabel))
            {
                logger.LogMissingRoomLabel();
                ModelState.AddModelError(nameof(request.RoomLabel), "Please provide a room label");
                return ValidationProblem(ModelState);
            }

            if (request.Answer != ConsultationAnswer.Accepted)
            {
                logger.LogConsultationAnswer(request.Answer.ToString());
                return NoContent();
            }

            var displayName = requestedFor.GetParticipantRoom()?.Label ?? requestedFor.DisplayName;
            var command = new SaveEventCommand(conference.Id, Guid.NewGuid().ToString(), EventType.Consultation,
                DateTime.UtcNow, null, null, $"Adding {displayName} to {request.RoomLabel}", null)
            {
                ParticipantId = request.RequestedBy
            };
            await commandHandler.Handle(command);
            await consultationService.ParticipantTransferToRoomAsync(request.ConferenceId, requestedFor.Id, request.RoomLabel);

            return NoContent();
        }
        
        /// <summary>
        /// Add an endpoint to a private consultation
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("endpoint")]
        [OpenApiOperation("JoinEndpointToConsultation")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> StartConsultationWithEndpointAsync(EndpointConsultationRequest request)
        {
            var isVhoRequest = request.RequestedById == Guid.Empty;
            var getConferenceByIdQuery = new GetConferenceByIdQuery(request.ConferenceId);
            var conference = await queryHandler.Handle<GetConferenceByIdQuery, Conference>(getConferenceByIdQuery);

            if (conference == null)
            {
                logger.LogConferenceNotFound(request.ConferenceId);
                return NotFound($"Unable to find conference {request.ConferenceId}");
            }

            var endpoint = conference.GetEndpoints().SingleOrDefault(x => x.Id == request.EndpointId);
            if (endpoint == null)
            {
                logger.LogEndpointNotFound(request.EndpointId);
                return NotFound($"Unable to find endpoint {request.EndpointId}");
            }

            var requestedBy = conference.GetParticipants().SingleOrDefault(x => x.Id == request.RequestedById);
            
            if (isVhoRequest 
                || requestedBy?.UserRole == UserRole.Judge
                || requestedBy?.UserRole == UserRole.StaffMember
                || requestedBy?.UserRole == UserRole.JudicialOfficeHolder)
            {
                await consultationService.EndpointTransferToRoomAsync(request.ConferenceId, endpoint.Id, request.RoomLabel);
                return Ok();
            }
            
            if (requestedBy == null)
            {
                logger.LogEndpointWithoutLinkedParticipant();
                return NotFound($"Unable to find requestedBy participant {request.RequestedById}");
            }
            
            if (endpoint.ParticipantsLinked == null || !endpoint.ParticipantsLinked.Any())
            {
                const string message = "Endpoint does not have a linked participant";
                logger.LogEndpointWithoutLinkedParticipant();
                return Unauthorized(message);
            }

            if (endpoint.ParticipantsLinked.All(p => p.Username != requestedBy.Username))
            {
                const string message = "Participant is not linked to requested endpoint";
                logger.LogParticipantNotLinkedToEndpoint();
                return Unauthorized(message);
            }

            var roomQuery = new GetConsultationRoomByIdQuery(request.ConferenceId, request.RoomLabel);
            var room = await queryHandler.Handle<GetConsultationRoomByIdQuery, ConsultationRoom>(roomQuery);
            if (room == null)
            {
                logger.LogRoomNotFound(request.RoomLabel);
                return NotFound($"Unable to find room {request.RoomLabel}");
            }

            if (room.RoomEndpoints.Count != 0)
            {
                logger.LogUnableToJoinEndpointToRoom(endpoint.Id, request.RoomLabel);
                ModelState.AddModelError("RoomLabel", "Room already has an active endpoint");
                return ValidationProblem(ModelState);
            }

            await consultationService.EndpointTransferToRoomAsync(request.ConferenceId, endpoint.Id, request.RoomLabel);
            return Ok();
        }
        
        [HttpPost("lockroom")]
        [OpenApiOperation("LockRoom")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> LockRoomRequestAsync(LockRoomRequest request)
        {
            var lockRoomCommand = new LockRoomCommand(request.ConferenceId, request.RoomLabel, request.Lock);
            await commandHandler.Handle(lockRoomCommand);
            return Ok();
        }
        
        [HttpPost("createconsultation")]
        [OpenApiOperation("CreatePrivateConsultation")]
        [ProducesResponseType(typeof(RoomResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> StartNewConsultationRequestAsync(StartConsultationRequest request)
        {
            var room = await consultationService.CreateNewConsultationRoomAsync(request.ConferenceId);
            await consultationService.ParticipantTransferToRoomAsync(request.ConferenceId, request.RequestedBy,
                room.Label);

            var response = RoomToDetailsResponseMapper.MapConsultationRoomToResponse(room);
            return Ok(response);
        }
        
        [HttpPost("start")]
        [OpenApiOperation("StartPrivateConsultation")]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> StartConsultationRequestAsync(StartConsultationRequest request)
        {
            var room = await consultationService.GetAvailableConsultationRoomAsync(request.ConferenceId,
                request.RoomType.MapToDomainEnum());
            await consultationService.ParticipantTransferToRoomAsync(request.ConferenceId, request.RequestedBy,
                room.Label);

            return Accepted();
        }
        
        /// <summary>
        /// Leave a consultation
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("leave")]
        [OpenApiOperation("LeaveConsultation")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> LeaveConsultationAsync(LeaveConsultationRequest request)
        {
            var getConferenceByIdQuery = new GetConferenceByIdQuery(request.ConferenceId);
            var conference = await queryHandler.Handle<GetConferenceByIdQuery, Conference>(getConferenceByIdQuery);

            if (conference == null)
            {
                logger.LogConferenceNotFound(request.ConferenceId);
                return NotFound();
            }

            var participant = conference.GetParticipants().SingleOrDefault(x => x.Id == request.ParticipantId);
            if (participant == null)
            {
                logger.LogUnableToFindParticipant(request.ParticipantId);
                return NotFound();
            }

            if (!participant.CurrentConsultationRoomId.HasValue)
            {
                ModelState.AddModelError("ParticipantId", "Participant is not in a consultation");
                return ValidationProblem(ModelState);
            }

            await consultationService.LeaveConsultationAsync(conference.Id, participant.Id, participant.GetCurrentRoom(), RoomType.WaitingRoom.ToString());
            return Ok();
        }
    }
}
