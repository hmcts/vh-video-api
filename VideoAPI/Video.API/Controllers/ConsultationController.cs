using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Domain.Validations;
using VideoApi.Events.Hub;
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
        private readonly IHubContext<EventHub, IEventHubClient> _hubContext;
        private readonly ILogger<ConsultationController> _logger;
        private readonly IVideoPlatformService _videoPlatformService;

        public ConsultationController(IQueryHandler queryHandler, ICommandHandler commandHandler,
            IHubContext<EventHub, IEventHubClient> hubContext, ILogger<ConsultationController> logger, IVideoPlatformService videoPlatformService)
        {
            _queryHandler = queryHandler;
            _commandHandler = commandHandler;
            _hubContext = hubContext;
            _logger = logger;
            _videoPlatformService = videoPlatformService;
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
            var getConferenceByIdQuery = new GetConferenceByIdQuery(request.ConferenceId);
            var conference =
                await _queryHandler.Handle<GetConferenceByIdQuery, Conference>(getConferenceByIdQuery);

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

            var requestRaised = !request.Answer.HasValue;
            if (requestRaised)
            {
                _logger.LogInformation(
                    $"Raising request between {requestedBy.Username} and {requestedFor.Username} in conference {conference.Id}");
                await NotifyConsultationRequest(conference, requestedBy, requestedFor);
            }
            else
            {
                _logger.LogInformation(
                    $"Answered request as {request.Answer.Value} between {requestedBy.Username} and {requestedFor.Username} in conference {conference.Id}");
                try
                {
                    await NotifyConsultationResponse(conference, requestedBy, requestedFor,
                        request.Answer.Value);
                }
                catch (DomainRuleException e)
                {
                    _logger.LogError(e, $"No consultation room available for conference {conference.Id}");
                    ModelState.AddModelError("ConsultationRoom", "No consultation room available");
                    return BadRequest(ModelState);
                }
            }

            return NoContent();
        }

        /// <summary>
        /// This method raises a notification to the requestee informing them of an incoming consultation request
        /// </summary>
        /// <param name="conference">The conference Id</param>
        /// <param name="requestedBy">The participant raising the consultation request</param>
        /// <param name="requestedFor">The participant with whom the consultation is being requested with</param>
        private async Task NotifyConsultationRequest(Conference conference, Participant requestedBy,
            Participant requestedFor)
        {
            await _hubContext.Clients.Group(requestedFor.Username.ToLowerInvariant())
                .ConsultationMessage(conference.Id, requestedBy.Username, requestedFor.Username,
                    string.Empty);
        }
        
        /// <summary>
        /// This method raises a notification to the requester informing them the response to their consultation request.
        /// </summary>
        /// <param name="conference">The conference Id</param>
        /// <param name="requestedBy">The participant raising the consultation request</param>
        /// <param name="requestedFor">The participant with whom the consultation is being requested with</param>
        /// /// <param name="answer">The answer to the request (i.e. Accepted or Rejected)</param>
        private async Task NotifyConsultationResponse(Conference conference, Participant requestedBy,
            Participant requestedFor, ConsultationAnswer answer)
        {
            await _hubContext.Clients.Group(requestedBy.Username.ToLowerInvariant())
                .ConsultationMessage(conference.Id, requestedBy.Username, requestedFor.Username, answer.ToString());
            
            if (answer == ConsultationAnswer.Accepted)
            {
                await _hubContext.Clients.Group(EventHub.VhOfficersGroupName)
                    .ConsultationMessage(conference.Id, requestedBy.Username, requestedFor.Username,
                        answer.ToString());
                
                await StartPrivateConsultationAsync(conference, requestedBy, requestedFor);
            }
        }

        private async Task StartPrivateConsultationAsync(Conference conference, Participant requestedBy, Participant requestedFor)
        {
            var targetRoom = conference.GetAvailableConsultationRoom();
            _logger.LogInformation(
                $"Conference: {conference.Id} - Attempting to transfer participants {requestedBy.Id} {requestedFor.Id} into room {targetRoom}");
            await _videoPlatformService.TransferParticipantAsync(conference.Id, requestedBy.Id,
                requestedBy.CurrentRoom.Value, targetRoom);
            
            await _videoPlatformService.TransferParticipantAsync(conference.Id, requestedFor.Id,
                requestedFor.CurrentRoom.Value, targetRoom);
        }
    }
}