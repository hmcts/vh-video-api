using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Swashbuckle.AspNetCore.Annotations;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Events.Hub;

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

        public ConsultationController(IQueryHandler queryHandler, ICommandHandler commandHandler,
            IHubContext<EventHub, IEventHubClient> hubContext)
        {
            _queryHandler = queryHandler;
            _commandHandler = commandHandler;
            _hubContext = hubContext;
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
            var getConferenceByIdQuery = new GetConferenceByIdQuery(request.ConferenceId);
            var conference =
                await _queryHandler.Handle<GetConferenceByIdQuery, Conference>(getConferenceByIdQuery);

            if (conference == null)
            {
                return NotFound();
            }

            var requestedBy = conference.GetParticipants().SingleOrDefault(x => x.Id == request.RequestedBy);
            if (requestedBy == null)
            {
                return NotFound();
            }

            var requestedFor = conference.GetParticipants().SingleOrDefault(x => x.Id == request.RequestedFor);
            if (requestedFor == null)
            {
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
                await NotifyConsultationRequest(conference, requestedBy, requestedFor);
            }
            else
            {
                await NotifyConsultationResponse(conference, requestedBy, requestedFor,
                    request.Answer.Value);
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
                var admin = conference.GetParticipants().Single(x => x.UserRole == UserRole.VideoHearingsOfficer);
                await _hubContext.Clients.Group(admin.Username.ToLowerInvariant())
                    .ConsultationMessage(conference.Id, requestedBy.Username, requestedFor.Username,
                        answer.ToString());
            }
        }
    }
}