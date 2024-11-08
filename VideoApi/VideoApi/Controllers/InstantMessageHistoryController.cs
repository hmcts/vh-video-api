using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using VideoApi.Contract.Responses;
using VideoApi.Mappings;
using VideoApi.Services;

namespace VideoApi.Controllers;

[Consumes("application/json")]
[Produces("application/json")]
[Route("conferences")]
[ApiController]
public class InstantMessageHistoryController(
    ILogger<InstantMessageHistoryController> logger,
    IInstantMessageService instantMessageService)
    : ControllerBase
{
    /// <summary>
    /// Get all the chat messages for a conference
    /// </summary>
    /// <param name="conferenceId">Id of the conference</param>
    /// <returns>Chat messages</returns>
    [HttpGet("{conferenceId}/instantmessages")]
    [OpenApiOperation("GetInstantMessageHistory")]
    [ProducesResponseType(typeof(List<InstantMessageResponse>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetInstantMessageHistoryAsync(Guid conferenceId)
    {
        logger.LogDebug("Retrieving instant message history");

        return await GetInstantMessageHistoryAsync(conferenceId, null);
    }

    /// <summary>
    /// Get all the chat messages for a conference
    /// </summary>
    /// <param name="conferenceId">Id of the conference</param>
    /// <param name="participantUsername">instant messages for the participant user name</param>
    /// <returns>Chat messages</returns>
    [HttpGet("{conferenceId}/instantMessages/{participantUsername}")]
    [OpenApiOperation("GetInstantMessageHistoryForParticipant")]
    [ProducesResponseType(typeof(List<InstantMessageResponse>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetInstantMessageHistoryForParticipantAsync(Guid conferenceId, string participantUsername)
    {
        logger.LogDebug("Retrieving instant message history");

        return await GetInstantMessageHistoryAsync(conferenceId, participantUsername);
    }
    
    private async Task<IActionResult> GetInstantMessageHistoryAsync(Guid conferenceId, string participantName)
    {
        logger.LogDebug("Retrieving instant message history");
        try
        {
            var messages =
                await instantMessageService.GetInstantMessagesForConferenceAsync(conferenceId, participantName);

            var response = messages.Select(InstantMessageToResponseMapper.MapMessageToResponse);
            return Ok(response);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to find instant messages");
            return NotFound();
        }
    }
}
