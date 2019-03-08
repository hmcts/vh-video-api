using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Video.API.Extensions;
using Video.API.Validations;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Models;

namespace Video.API.Controllers
{
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("callback")]
    [ApiController]
    public class CallbackController : ControllerBase
    {
        private readonly IQueryHandler _queryHandler;
        private readonly ICommandHandler _commandHandler;
        private readonly IEventHandlerFactory _eventHandlerFactory;

        public CallbackController(IQueryHandler queryHandler, ICommandHandler commandHandler,
            IEventHandlerFactory eventHandlerFactory)
        {
            _queryHandler = queryHandler;
            _commandHandler = commandHandler;
            _eventHandlerFactory = eventHandlerFactory;
        }

        /// <summary>
        /// Accepts video conference events and publishes to internal clients/services
        /// </summary>
        /// <param name="request">Details of the event</param>
        /// <returns>NoContent if event is handled as expected</returns>
        [HttpPost("conference")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> PostEvents(ConferenceEventRequest request)
        {
            var result = await new ConferenceEventRequestValidation().ValidateAsync(request);
            if (!result.IsValid)
            {
                ModelState.AddFluentValidationErrors(result.Errors);
                return BadRequest(ModelState);
            }
            
            var getConferenceByIdQuery = new GetConferenceByIdQuery(Guid.Parse(request.ConferenceId));
            var queriedConference =
                await _queryHandler.Handle<GetConferenceByIdQuery, Conference>(getConferenceByIdQuery);

            if (queriedConference == null)
            {
                return NotFound();
            }
            
            Guid.TryParse(request.ParticipantId, out var participantId);
            var participant = queriedConference.GetParticipants().SingleOrDefault(x => x.Id == participantId);
            if (participant == null && request.EventType != EventType.Pause && 
                request.EventType != EventType.Close && request.EventType != EventType.Help)
            {
                return NotFound();
            }

            var command = new SaveEventCommand(request.EventId, request.EventType, request.TimeStampUtc, participantId,
                request.TransferFrom, request.TransferTo, request.Reason);
            
            await _commandHandler.Handle(command);
            var callbackEvent = new CallbackEvent
            {
                EventId = request.EventId,
                EventType = request.EventType,
                ConferenceId = queriedConference.Id,
                Reason = request.Reason,
                TransferTo = request.TransferTo,
                TransferFrom = request.TransferFrom,
                TimeStampUtc = request.TimeStampUtc,
                ParticipantId = participantId
            };
            
            await _eventHandlerFactory.Get(request.EventType).HandleAsync(callbackEvent);
            return Ok();
        }
    }
}