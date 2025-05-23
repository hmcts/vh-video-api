using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
using VideoApi.Mappings;
using VideoApi.Services;
using VideoApi.Common.Logging;

namespace VideoApi.Controllers
{
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("conferences")]
    [ApiController]
    public class InstantMessageController(
        ILogger<InstantMessageController> logger,
        IInstantMessageService instantMessageService)
        : ControllerBase
    {
        /// <summary>
        /// Saves chat message exchanged between participants
        /// </summary>
        /// <param name="conferenceId">Id of the conference</param>
        /// <param name="request">Details of the chat message</param>
        /// <returns>OK if the message is saved successfully</returns>
        [HttpPost("{conferenceId}/instantmessages")]
        [OpenApiOperation("AddInstantMessageToConference")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddInstantMessageToConferenceAsync(Guid conferenceId, AddInstantMessageRequest request)
        {
            logger.LogSavingInstantMessage();

            try
            {
                await instantMessageService.AddInstantMessageAsync(conferenceId, request.From, request.MessageText,
                    request.To);

                return Ok("InstantMessage saved");
            }
            catch (Exception e)
            {
                logger.LogUnableToAddInstantMessages(e);
                return BadRequest();
            }
        }

        [HttpDelete("{conferenceId}/instantmessages")]
        [OpenApiOperation("RemoveInstantMessages")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> RemoveInstantMessagesForConferenceAsync(Guid conferenceId)
        {
            logger.LogRemovingInstantMessagesForConference();
            try
            {
                await instantMessageService.RemoveInstantMessagesForConferenceAsync(conferenceId);

                logger.LogInstantMessageDeleted();
                return Ok();
            }
            catch (Exception e)
            {
                logger.LogUnableToRemoveInstantMessages(e);
                return BadRequest();
            }
        }

        /// <summary>
        /// Get list of closed conferences with instant messages (closed over 30 minutes ago)
        /// </summary>
        /// <returns>List of Conference Ids</returns>
        [HttpGet("expiredIM")]
        [OpenApiOperation("GetClosedConferencesWithInstantMessages")]
        [ProducesResponseType(typeof(List<ClosedConferencesResponse>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetClosedConferencesWithInstantMessagesAsync()
        {
            logger.LogGettingClosedConferencesWithInstantMessages();
            var closedConferences = await instantMessageService.GetClosedConferencesWithInstantMessages();

            if (closedConferences.Count == 0)
            {
                logger.LogNoClosedConferencesWithInstantMessagesFound();
                return Ok(new List<ClosedConferencesResponse>());
            }

            var response = closedConferences.Select(ConferenceToClosedConferenceMapper.MapConferenceToClosedResponse);
            return Ok(response);
        }
    }
}
