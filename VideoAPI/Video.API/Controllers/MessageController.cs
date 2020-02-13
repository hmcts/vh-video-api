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
    public class MessageController : ControllerBase
    {
        private readonly IQueryHandler _queryHandler;
        private readonly ICommandHandler _commandHandler;
        private readonly ILogger<MessageController> _logger;

        public MessageController(IQueryHandler queryHandler, ICommandHandler commandHandler,ILogger<MessageController> logger)
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
        [HttpGet("{conferenceId}/messages")]
        [SwaggerOperation(OperationId = "GetMessages")]
        [ProducesResponseType(typeof(List<MessageResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetMessages(Guid conferenceId)
        {
            _logger.LogDebug("GetMessages");
            var query = new GetMessagesForConferenceQuery(conferenceId);
            try
            {
                var messages = await _queryHandler.Handle<GetMessagesForConferenceQuery, List<Message>>(query);
                var mapper = new MessageToResponseMapper();
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
        [HttpPost("{conferenceId}/messages")]
        [SwaggerOperation(OperationId = "SaveMessage")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SaveMessage(Guid conferenceId, AddMessageRequest request)
        {
            _logger.LogInformation($"Saving chat message for conference {conferenceId}");

            try
            {
                var command = new AddMessageCommand(conferenceId, request.From, request.To, request.MessageText);
                await _commandHandler.Handle(command);

                return Ok("Message saved");
            }
            catch (ConferenceNotFoundException)
            {
                _logger.LogError($"Unable to find conference {conferenceId}");
                return NotFound();
            }
            catch (ParticipantNotFoundException)
            {
                _logger.LogError($"One of the participant does not exist in the conference {conferenceId}");
                return BadRequest();
            }
        }
    }
}
