using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using Video.API.Mappings;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;

namespace Video.API.Controllers
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

        public InstantMessageController(IQueryHandler queryHandler, ICommandHandler commandHandler,
            ILogger<InstantMessageController> logger)
        {
            _queryHandler = queryHandler;
            _commandHandler = commandHandler;
            _logger = logger;
        }

        /// <summary>
        /// Get all the chat messages for a conference
        /// </summary>
        /// <param name="conferenceId">Id of the conference</param>
        /// <returns>Chat messages</returns>
        [HttpGet("{conferenceId}/instantmessages")]
        [SwaggerOperation(OperationId = "GetInstantMessageHistory")]
        [ProducesResponseType(typeof(List<InstantMessageResponse>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetInstantMessageHistory(Guid conferenceId)
        {
            _logger.LogDebug($"Retrieving instant message history for conference {conferenceId}");
            var query = new GetInstantMessagesForConferenceQuery(conferenceId);
            try
            {
                var messages =
                    await _queryHandler.Handle<GetInstantMessagesForConferenceQuery, List<InstantMessage>>(query);
                var mapper = new InstantMessageToResponseMapper();
                var response = messages.Select(mapper.MapMessageToResponse);
                return Ok(response);
            }
            catch (ConferenceNotFoundException)
            {
                _logger.LogError($"Unable to find conference {conferenceId}");
                return NotFound();
            }
        }

        /// <summary>
        /// Saves chat message exchanged between participants
        /// </summary>
        /// <param name="conferenceId">Id of the conference</param>
        /// <param name="request">Details of the chat message</param>
        /// <returns>OK if the message is saved successfully</returns>
        [HttpPost("{conferenceId}/instantmessages")]
        [SwaggerOperation(OperationId = "AddInstantMessageToConference")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddInstantMessageToConference(Guid conferenceId, AddInstantMessageRequest request)
        {
            _logger.LogDebug($"Saving instant message for conference {conferenceId}");

            try
            {
                var command = new AddInstantMessageCommand(conferenceId, request.From, request.MessageText);
                await _commandHandler.Handle(command);

                return Ok("InstantMessage saved");
            }
            catch (ConferenceNotFoundException)
            {
                _logger.LogError($"Unable to find conference {conferenceId}");
                return NotFound();
            }
        }

        /// <summary>
        /// Get list of closed conferenceswith instant messages (closed over 30 minutes ago)
        /// </summary>
        /// <returns>List of Conference Ids</returns>
        [HttpGet("expiredIM")]
        [SwaggerOperation(OperationId = "GetClosedConferencesWithInstantMessages")]
        [ProducesResponseType(typeof(List<ClosedConferencesResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetClosedConferencesWithInstantMessages()
        {
            _logger.LogDebug($"GetClosedConferences");
            var query = new GetClosedConferencesWithInstantMessagesQuery();
            var closedConferences = await _queryHandler.Handle<GetClosedConferencesWithInstantMessagesQuery, List<Conference>>(query);

            if (closedConferences == null)
            {
                _logger.LogDebug($"No Closed Clonferences found");
                return NotFound(new List<ClosedConferencesResponse>());
            }

            var mapper = new ConferenceToClosedConferenceMapper();
            var response = closedConferences.Select(mapper.MapConferenceToClosedResponse);
            return Ok(response);
        }
    }
}
