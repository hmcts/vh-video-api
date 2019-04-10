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

            var admin = conference.Participants.Single(x => x.UserRole == UserRole.VideoHearingsOfficer);
            switch (request.Answer)
            {
                case null:
                    await _hubContext.Clients.Group(requestedFor.Username.ToLowerInvariant())
                        .ConsultationMessage(conference.Id, requestedBy.Username, requestedFor.Username,
                            string.Empty);
                    break;
                case ConsultationAnswer.Accepted:
                    await _hubContext.Clients.Group(admin.Username.ToLowerInvariant()).ConsultationMessage(
                        conference.Id,
                        requestedBy.Username, requestedFor.Username, request.Answer.ToString());
                    break;
            }

            return NoContent();
        }
    }
}