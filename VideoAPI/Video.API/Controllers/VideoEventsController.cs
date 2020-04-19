using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Models;

namespace Video.API.Controllers
{
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("events")]
    [ApiController]
    public class VideoEventsController : ControllerBase
    {
        private readonly ICommandHandler _commandHandler;
        private readonly IEventHandlerFactory _eventHandlerFactory;
        private readonly ILogger<VideoEventsController> _logger;

        public VideoEventsController(ICommandHandler commandHandler, IEventHandlerFactory eventHandlerFactory,
            ILogger<VideoEventsController> logger)
        {
            _commandHandler = commandHandler;
            _eventHandlerFactory = eventHandlerFactory;
            _logger = logger;
        }

        /// <summary>
        /// Accepts video conference events and publishes to internal clients/services
        /// </summary>
        /// <param name="request">Details of the event</param>
        /// <returns>NoContent if event is handled as expected</returns>
        [HttpPost]
        [SwaggerOperation(OperationId = "RaiseVideoEvent")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> PostEventAsync(ConferenceEventRequest request)
        {
            _logger.LogInformation($"Handling {request.EventType.ToString()} event for conference {request.ConferenceId}");
            Guid.TryParse(request.ConferenceId, out var conferenceId);

            var command = new SaveEventCommand(conferenceId, request.EventId, request.EventType,
                request.TimeStampUtc, request.TransferFrom, request.TransferTo, request.Reason);
            if (Guid.TryParse(request.ParticipantId, out var participantId))
            {
                command.ParticipantId = participantId;
            }

            _logger.LogWarning($"EVENT: {request.EventType.ToString()} | " +
                               $"Participant ID: {request.ParticipantId} | " +
                               $"Reason: {request.Reason} | " +
                               $"External Timestamp: {request.TimeStampUtc:yyyy-MM-dd HH:mm:ss.fffffff} | " +
                               $"Timestamp: {(DateTime.Now):yyyy-MM-dd HH:mm:ss.fffffff} | " +
                               $"TransferFrom: {request.TransferFrom} | " +
                               $"TransferTo: {request.TransferTo} | " +
                               $"Conference ID: {request.ConferenceId} ");
            
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
