using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Swashbuckle.AspNetCore.Annotations;
using Video.API.Extensions;
using Video.API.Validations;
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
        /// Raise a private consultation request with another participant
        /// </summary>
        /// <param name="request">Details of participants to put into a private consultation</param>
        /// <returns></returns>
        [HttpPost("request")]
        [SwaggerOperation(OperationId = "RaiseConsultationRequest")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> RaiseConsultationRequest(ConsultationRequest request)
        {
            var result = await new ConsultationRequestValidation().ValidateAsync(request);
            if (!result.IsValid)
            {
                ModelState.AddFluentValidationErrors(result.Errors);
                return BadRequest(ModelState);
            }
            
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
            
            await _hubContext.Clients.Group(requestedFor.Username.ToLowerInvariant())
                .ConsultationMessage(conference.Id, requestedBy.Username, requestedFor.Username,
                    string.Empty);

            return NoContent();
        }
        
        /// <summary>
        /// Provide an answer to a private consultation request with another participant
        /// </summary>
        /// <param name="request">Details of the PC request with the requestee's answer</param>
        /// <returns></returns>
        [HttpPost("respond")]
        [SwaggerOperation(OperationId = "AnswerConsultationRequest")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AnswerConsultationRequest(ConsultationResultRequest request)
        { 
            var result = await new ConsultationResultRequestValidation().ValidateAsync(request);
            if (!result.IsValid)
            {
                ModelState.AddFluentValidationErrors(result.Errors);
                return BadRequest(ModelState);
            }
            
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
            
            await _hubContext.Clients.Group(requestedFor.Username.ToLowerInvariant())
                .ConsultationMessage(conference.Id, requestedBy.Username, requestedFor.Username,
                    request.Answer.ToString());
            
            if (request.Answer == ConsultationRequestAnswer.Accepted)
            {
                var admin = conference.Participants.Single(x => x.UserRole == UserRole.VideoHearingsOfficer);
                await _hubContext.Clients.Group(admin.Username.ToLowerInvariant()).ConsultationMessage(conference.Id,
                    requestedBy.Username, requestedFor.Username, request.Answer.ToString());
            }

            return NoContent();
        }
        
    }
}