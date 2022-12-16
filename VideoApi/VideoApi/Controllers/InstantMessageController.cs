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
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Mappings;

namespace VideoApi.Controllers
{
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("conferences")]
    [ApiController]
    public class InstantMessageController : ControllerBase
    {
        private readonly IQueryHandler _queryHandler;
        private readonly ICommandHandler _commandHandler;
        private readonly ILogger<InstantMessageController> _logger;
        private readonly IBackgroundWorkerQueue _backgroundWorkerQueue;

        public InstantMessageController(IQueryHandler queryHandler, ICommandHandler commandHandler,
            ILogger<InstantMessageController> logger, IBackgroundWorkerQueue backgroundWorkerQueue)
        {
            _queryHandler = queryHandler;
            _commandHandler = commandHandler;
            _logger = logger;
            _backgroundWorkerQueue = backgroundWorkerQueue;
        }

        /// <summary>
        /// Get all the chat messages for a conference
        /// </summary>
        /// <param name="conferenceId">Id of the conference</param>
        /// <returns>Chat messages</returns>
        [HttpGet("{conferenceId}/instantmessages")]
        [OpenApiOperation("GetInstantMessageHistory")]
        [ProducesResponseType(typeof(List<InstantMessageResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetInstantMessageHistoryAsync(Guid conferenceId)
        {
            _logger.LogDebug("Retrieving instant message history");
            var query = new GetInstantMessagesForConferenceQuery(conferenceId, null);

            return await GetInstantMessageHistoryAsync(query);
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
            _logger.LogDebug("Retrieving instant message history");
            var query = new GetInstantMessagesForConferenceQuery(conferenceId, participantUsername);

            return await GetInstantMessageHistoryAsync(query);
        }

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
            _logger.LogDebug("Saving instant message");

            try
            {
                var command = new AddInstantMessageCommand(conferenceId, request.From, request.MessageText, request.To);
                await _commandHandler.Handle(command);

                return Ok("InstantMessage saved");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to add instant messages");
                return BadRequest();
            }
        }

        [HttpDelete("{conferenceId}/instantmessages")]
        [OpenApiOperation("RemoveInstantMessages")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> RemoveInstantMessagesForConferenceAsync(Guid conferenceId)

        {
            _logger.LogDebug("RemoveInstantMessagesForConference");
            try
            {
                var command = new RemoveInstantMessagesForConferenceCommand(conferenceId);
                await _backgroundWorkerQueue.QueueBackgroundWorkItem(command);

                _logger.LogDebug("InstantMessage deleted");
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to remove instant messages");
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
            _logger.LogDebug($"GetClosedConferencesWithInstantMessages");
            var query = new GetClosedConferencesWithInstantMessagesQuery();
            var closedConferences = await _queryHandler.Handle<GetClosedConferencesWithInstantMessagesQuery, List<Conference>>(query);

            if (!closedConferences.Any())
            {
                _logger.LogDebug($"No closed conferences with instant messages found.");
                return Ok(new List<ClosedConferencesResponse>());
            }

            var response = closedConferences.Select(ConferenceToClosedConferenceMapper.MapConferenceToClosedResponse);
            return Ok(response);
        }

        private async Task<IActionResult> GetInstantMessageHistoryAsync(GetInstantMessagesForConferenceQuery query)
        {
            _logger.LogDebug("Retrieving instant message history");
            try
            {
                var messages =
                    await _queryHandler.Handle<GetInstantMessagesForConferenceQuery, List<InstantMessage>>(query);

                var response = messages.Select(InstantMessageToResponseMapper.MapMessageToResponse);
                return Ok(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to find instant messages");
                return NotFound();
            }
        }
    }
}
