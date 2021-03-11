using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Commands.Core;
using VideoApi.Events.Handlers.Core;
using VideoApi.Extensions;
using VideoApi.Mappings;

namespace VideoApi.Controllers
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
        [OpenApiOperation("RaiseVideoEvent")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> PostEventAsync(ConferenceEventRequest request)
        {
            Guid.TryParse(request.ConferenceId, out var conferenceId);
            Guid.TryParse(request.ParticipantId, out var participantId);

            var command = EventRequestMapper.MapEventRequestToEventCommand(conferenceId, request);

            _logger.LogWarning("Handling {ConferenceEventRequest}", nameof(ConferenceEventRequest));
            
            await _commandHandler.Handle(command);

            if (request.ShouldSkipEventHandler())
            {
                _logger.LogDebug("Handling CallbackEvent skipped due to result of ShouldHandleEvent");
                return NoContent();
            }

            var callbackEvent = EventRequestMapper.MapEventRequestToEventHandlerDto(conferenceId, participantId, request);
            await _eventHandlerFactory.Get(callbackEvent.EventType).HandleAsync(callbackEvent);
            return NoContent();
        }
    }
}
