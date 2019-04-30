using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
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
        private readonly ICommandHandler _commandHandler;
        private readonly IEventHandlerFactory _eventHandlerFactory;

        public CallbackController(ICommandHandler commandHandler, IEventHandlerFactory eventHandlerFactory)
        {
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
            Guid.TryParse(request.ConferenceId, out var conferenceId);

            var command = new SaveEventCommand(conferenceId, request.EventId, request.EventType,
                request.TimeStampUtc, request.TransferFrom, request.TransferTo, request.Reason);
            if (Guid.TryParse(request.ParticipantId, out var participantId))
            {
                command.ParticipantId = participantId;
            }

            await _commandHandler.Handle(command);

            var callbackEvent = new CallbackEvent
            {
                EventId = request.EventId,
                EventType = request.EventType,
                ConferenceId = conferenceId,
                Reason = request.Reason,
                TransferTo = request.TransferTo,
                TransferFrom = request.TransferFrom,
                TimeStampUtc = request.TimeStampUtc,
                ParticipantId = participantId
            };

            await _eventHandlerFactory.Get(request.EventType).HandleAsync(callbackEvent);
            return NoContent();
        }
    }
}