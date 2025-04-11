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

namespace VideoApi.Controllers;

[AllowAnonymous]
[Consumes("application/json")]
[Produces("application/json")]
[Route("events")]
[ApiController]
public class VideoEventsController(
    ICommandHandler commandHandler,
    IEventHandlerFactory eventHandlerFactory,
    ILogger<VideoEventsController> logger)
    : ControllerBase
{
    /// <summary>
    /// Accepts video conference events and publishes to internal clients/services
    /// </summary>
    /// <param name="request">Details of the event</param>
    /// <returns>NoContent if event is handled as expected</returns>
    [HttpPost]
    [OpenApiOperation("RaiseVideoEvent")]
    [ProducesResponseType((int) HttpStatusCode.NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails),(int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> PostEventAsync(ConferenceEventRequest request)
    {
#pragma warning disable CA1806
        Guid.TryParse(request.ConferenceId, out var conferenceId);
        Guid.TryParse(request.ParticipantId, out var participantId);
#pragma warning restore CA1806
        
        var command = EventRequestMapper.MapEventRequestToEventCommand(conferenceId, request);
        
        
        await commandHandler.Handle(command);
        
        if (request.ShouldSkipEventHandler())
        {
            logger.LogTrace("Handling CallbackEvent ({@Request}) skipped due to result of ShouldHandleEvent", request);
            return NoContent();
        }
        
        var callbackEvent = EventRequestMapper.MapEventRequestToEventHandlerDto(conferenceId, participantId, request);
        await eventHandlerFactory.Get(callbackEvent.EventType).HandleAsync(callbackEvent);
        return NoContent();
    }
}
